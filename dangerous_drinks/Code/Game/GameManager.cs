using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Sandbox;

public enum GameState {PreGame, GameStart, TurnSelect, NewRound, RoundStart, RoundEnd, GameEnd, Error}
public sealed class GameManager : Component
{
	/// <summary>
	/// GAME LOGIC AND VARIABLES
	/// </summary>

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
		if(a.hasBottles)a.KillBottles();
		a.SpawnBottles();
		Log.Info("NEW ROUND STARTED");
		CurrentGameState = GameState.TurnSelect;

	}
	private void OnTurnSelect()
	{
		stateTimer = 4f;
		if(CurrentPlayer > 1) CurrentPlayer = 0;
		else if(CurrentPlayer == 0) CurrentPlayer = 1;
		else if(CurrentPlayer == 1) CurrentPlayer = 0;
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
		var poison = (!GetComponent<BottleLogic>().HasPoison || DrankPoison);
		var bottle = (GetComponent<BottleLogic>().hasBottles);
		var dead = false;

		for (int i = 0; i < Players.Length; i++) //End Game if lives are gone.
		{
			if( Players[i].Lives <= 0)
			{
				dead = true;
				break;
			}
		}

		if (dead)
		{
			Log.Info("Player has died.");
			CurrentGameState = GameState.GameEnd;
		}
		else if (poison)
		{
			Log.Info("Player " + CurrentPlayer + " drank poison.");
			CurrentGameState = GameState.NewRound;
		}
		else if (!bottle)
		{
			Log.Info("Bottles Finished.");
			CurrentGameState = GameState.NewRound;
		}
		else
		CurrentGameState = GameState.TurnSelect;
		
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
	private void AttachCamera(int Player)
	{
		var playerRelay = PlayerAsset[Player].GetComponent<PlayerRelay>();
		var camera = Scene.Camera;
		camera.GameObject.Parent = playerRelay.EyeSlot;
		camera.GameObject.LocalPosition = Vector3.Zero;
		camera.GameObject.LocalRotation = Rotation.Identity;
	}
}

/// <summary>
/// PLAYER UNIVERSAL DATA AND VARIABLES
/// </summary>

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
