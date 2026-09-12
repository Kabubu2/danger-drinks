using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Sandbox;

public enum GameState {PreGame, GameStart, TurnSelect, NewRound, RoundStart, RoundEnd, GameEnd, Error}
public enum CourseState {None, Empty, Clear, Poison, Death, Error}
public sealed class GameManager : Component
{
	/// ----------------------------------------
	/// GAME LOGIC AND VARIABLES
	/// ----------------------------------------

	[Property]
	public GameObject Bottles;
	[Property]
	public GameObject WaitPoint;
	[Property]
	public PostProcessController PPC;
	private TimeUntil stateTimer = 4f;
	protected override void OnUpdate()
	{
		switch (CurrentGameState)
		{
			case GameState.PreGame:
			OnPreGame();
			break;
			case GameState.GameStart:
			OnGameStart();
			break;
			case GameState.TurnSelect:
			OnTurnSelect();
			break;
			case GameState.NewRound:
			OnNewRound();
			break;
			case GameState.RoundStart:
			OnRoundStart();
			break;
			case GameState.RoundEnd:
			OnRoundEnd();
			break;
			case GameState.GameEnd:
			OnGameEnd();
			break;
			case GameState.Error:
			Log.Error("Someway... somehow.. the game state slipped off the rails. If this happens we need to figure out how.");
			break;
			default:
			Log.Error("Someway... somehow.. the game state slipped off the rails. If this happens we need to figure out how.");
			break;
		}
	}
	[Property]
	private int CurrentPlayer{ get; set; } = 2;
	private GameState CurrentGameState{get; set;}

	public bool CanAct(GameObject player)
	{
		for (int i = 0; i < Players.Length; i++)
		{
			if (Players[i].PlayerInst == player && CurrentPlayer == i && CurrentGameState == GameState.RoundStart)
			{
				return true;
			}
		}
		return false;
	}
	private bool DrankPoison = false;
	public void PlayerInteract(GameObject player, GameObject item)
	{
		if(CanAct(player))
		{
			bool isPotion = item.GetComponent<Bottle>().IsValid;
			bool isPoison = false;
			if(isPotion) isPoison = item.GetComponent<Bottle>().poisoned;

			for (int i = 0; i < Players.Length; i++)
			{
				if (Players[i].PlayerInst == player && CurrentPlayer == i)
				{
					if(isPotion)
					{
						if(isPoison) 
						{
							DrankPoison = true;
							Players[i].Lives--;
							Players[i].PoisonsDrank += 1;
						}
						if(!isPoison)
						{
							Players[i].DudsDrank++;
						}
						item.Destroy();
						CurrentGameState = GameState.RoundEnd;
					}
					//Do something else: like ability or nothing
				}
			}
		}
	}


	/// ----------------------------------------
	/// COURSE LOGIC
	/// ----------------------------------------



	private CourseState Course()
	{

			if(isDead()) return CourseState.Death;
			else if(!GetComponent<BottleLogic>().HasPoison()) return CourseState.Clear;
			else if(DrankPoison) return CourseState.Poison;
			else if(!GetComponent<BottleLogic>().hasBottles()) return CourseState.Empty;
			else return CourseState.None;
			//return CourseState.Error;
		}



	/// ----------------------------------------
	/// ROUND LOGIC
	/// ----------------------------------------
	private void OnPreGame() //FIRES BEFORE ANYONE CAN SEE ANYTHING OR DO ANYTHING.
	{
		//Preloading
		AssignJoinID();
		PPC.Trigger(true);
		//Before Setting Camera
		if(stateTimer > 0) return;
		AttachCamera(0);
		CurrentGameState = GameState.GameStart;
	}
	private void OnGameStart() //FIRES WHEN PREGAME IS DONE AND THE PLAYERS ARE SUCCESSFULLY PLACED.
	{
		foreach (GameObject Player in PlayerAsset)
		{
			Player.GetComponent<AnimManager>().anim_Sit();
		}
		PPC.Trigger(false);
		Mouse.Visibility = MouseVisibility.Visible; //Display Mouse

		CurrentGameState = GameState.NewRound; //Transition to RoundStart
	}
	private void OnNewRound() //FIRES WHEN A NEW ROUND BEGINS.
	{
		var a = GetComponent<BottleLogic>();
		a.SpawnBottles();
		Log.Info("NEW ROUND STARTED");
		CurrentGameState = GameState.TurnSelect;

	}
	private void OnTurnSelect()
	{
		stateTimer = 4f;
		switch(CurrentPlayer)
		{
			case 0:
			CurrentPlayer = 1;
			break;
			case 1:
			CurrentPlayer = 0;
			break;
			default:
			CurrentPlayer = 0;
			break;
		}

		CurrentGameState = GameState.RoundStart;

	}
	private void OnRoundStart() //FIRES WHEN GAMESTART IS OVER AND ROUND HAS BEGUN.
	{
		Log.Info("Round Started for Player: " + CurrentPlayer);
		//Player must choose bottle and drink

	}
	private void OnRoundEnd()
	{
		Log.Info("Round Ended for Player: " + CurrentPlayer);

		switch (Course())
		{
			case CourseState.Clear:
			Log.Info("Course: No More Poison");
			CurrentGameState = GameState.NewRound;
			break;
			case CourseState.Death:
			Log.Info("Course: Death");
			CurrentGameState = GameState.GameEnd;
			break;
			case CourseState.Empty:
			Log.Info("Course: No More Bottles");
			CurrentGameState = GameState.NewRound;
			break;
			case CourseState.Poison:
			Log.Info("Course: Poison Drank");
			CurrentGameState = GameState.NewRound;
			break;
			case CourseState.None:
			CurrentGameState = GameState.TurnSelect;
			break;
			case CourseState.Error:
			Log.Warning("Course: State Slipped through the rails. Track the error.");
			CurrentGameState = GameState.NewRound;
			break;
			default:
			break;
		}
		
	}
	private void OnGameEnd()
	{
		int deadman = 2;
		for (int i = 0; i < Players.Length; i++)
		{
			if(Players[i].Lives <= 0) 
			{
				deadman = i;
				break;
			}
		}
		Log.Info("Game Over! Player " + deadman + " has lost the game.");
		//kill player with 0 lives.
		//maybe a stat screen?
	}
	/// ----------------------------------------
	/// PLAYER SYSTEMS
	/// ----------------------------------------

	[Property]
	public GameObject PlayerRef{ get; set; } //REFERENCE OBJECT FOR PLAYER INSTANTIATION
	[Property]
 	public GameObject[] PlayerSpawnPoints { get; set; } //SPAWNPOINTS FOR PLAYER INSTANCES
	private PlayerData[] Players = new PlayerData[2];	//PLAYER LIST FOR EASY REFERENCE
	private GameObject[] PlayerAsset = new GameObject[2]; //PLAYER GAMEOBJECT LIST FOR EASY MANIPULATION

	private void AssignJoinID() //CHECKS EACH AVAILABLE SLOT FOR PLAYERS TO FILL. MUST ADD FALLBACK FOR FULL LOBBY.
	{
		if(PlayerAsset[0] == null) InstancePlayerAsset(0);
		if(PlayerAsset[1] == null) InstancePlayerAsset(1);
	}
	private void InstancePlayerAsset(int PlayerInt) //SETS THE NECESSARY DETAILS FOR EACH PLAYER ALONG WITH THE SPAWNING OF A PLAYER.
	{
		Players[PlayerInt] = new PlayerData(); 
		Players[PlayerInt].TeamState = Team.Player;
		Players[PlayerInt].Lives = 3;
		Players[PlayerInt].LivesMax = 3;
		Players[PlayerInt].DudsDrank = 0;
		Players[PlayerInt].PoisonsDrank = 0;
		PlayerAsset[PlayerInt] = PlayerRef.Clone();
		PlayerAsset[PlayerInt].Parent = PlayerSpawnPoints[PlayerInt];
		PlayerAsset[PlayerInt].LocalPosition = Vector3.Zero;
		PlayerAsset[PlayerInt].LocalRotation = Rotation.Identity;
		Players[PlayerInt].PlayerInst = PlayerAsset[PlayerInt];
		
	}
	private bool isDead(out GameObject player)
	{
		for (int i = 0; i < Players.Length; i++) //End Game if lives are gone.
		{
			if( Players[i].Lives <= 0)
			{
				player = PlayerAsset[i];
				return true;
			}
		}
		player = null;
		return false;
	}
	private bool isDead()
	{
		for (int i = 0; i < Players.Length; i++) //End Game if lives are gone.
		{
			if( Players[i].Lives <= 0) return true;
		}
		return false;
	}
	private void AttachCamera(int Player)
	{
		var playerRelay = PlayerAsset[Player].GetComponent<PlayerRelay>();
		var camera = Scene.Camera;
		camera.GameObject.Parent = playerRelay.EyeSlot;
		camera.GameObject.LocalPosition = Vector3.Zero;
		camera.GameObject.LocalRotation = Rotation.Identity;
	}
}

/// ----------------------------------------
/// PLAYER UNIVERSAL DATA AND VARIABLES
/// ----------------------------------------

public enum Team { Player, Opponant, Neither }
public class UserData
{
	//SteamID
	//PlayerName
	//Player Slot
}
public class PlayerData
{
	public UserData UserID {get; set;}
	public Team TeamState { get; set; }
	public int Lives{ get; set; }
	public int LivesMax{ get; set; }
	public int DudsDrank{ get; set; }
	public int PoisonsDrank{ get; set; }
	public GameObject PlayerInst{ get; set; }
}
