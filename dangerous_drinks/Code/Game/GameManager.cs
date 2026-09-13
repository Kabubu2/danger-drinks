using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Sandbox;

public enum GameState {None, PreGame, GameStart, PreRound, RoundStart, PreTurn, TurnStart, TurnHold, TurnEnd, PostTurn, RoundEnd, PostRound, GameEnd, PostGame}
public enum CourseState {None, NoBottles, NoPoisons, DrankPoison, DrankNormal}
public sealed class GameManager : Component
{
	/// ----------------------------------------
	/// GAME LOGIC AND VARIABLES
	/// ----------------------------------------

	[Property]
	public GameObject BottlesContainer;
	[Property]
	public GameObject WaitPoint;
	[Property]
	public PostProcessController PostProcessCon{ get; set; }


	protected override void OnUpdate()
	{
		if(Input.Pressed("Use"))CurrentGameState = GameState.PreGame; //Temporary Start Method
		Logic();
	}

	private void Logic()
	{
		switch (CurrentGameState)
		{
			case GameState.PreGame:
			OnPreGame();
			break;
			case GameState.GameStart:
			OnGameStart();
			break;

			case GameState.PreRound:
			OnPreRound();
			break;
			case GameState.RoundStart:
			OnRoundStart();
			break;

			case GameState.PreTurn:
			OnPreTurn();
			break;
			case GameState.TurnStart:
			OnTurnStart();
			break;
			case GameState.TurnHold:
			OnTurnHold();
			break;
			case GameState.TurnEnd:
			OnTurnEnd();
			break;
			case GameState.PostTurn:
			OnPostTurn();
			break;

			case GameState.RoundEnd:
			OnRoundEnd();
			break;
			case GameState.PostRound:
			OnPostRound();
			break;

			case GameState.GameEnd:
			OnGameEnd();
			break;
			case GameState.PostGame:
			OnPostGame();
			break;

			case GameState.None:
			break;
			default:
			break;
		}
	}

	[Property]
	private GameState CurrentGameState{ get; set; } //The current state of the game.

	/// ----------------------------------------
	/// COURSE LOGIC
	/// ----------------------------------------

	private bool DrankPoison;
	private void Course(CourseState course)
	{
		switch (course)
		{
			case CourseState.NoBottles:
			Course_NoBottles();
			break;
			case CourseState.NoPoisons:
			Course_NoPoisons();
			break;
			case CourseState.DrankNormal:
			Course_DrankNormal();
			break;
			case CourseState.DrankPoison:
			Course_DrankPoison();
			break;
			case CourseState.None:
			break;
			default:
			break;
		}
	}
	private void Course_NoBottles()
	{
		CurrentGameState = GameState.RoundEnd;
	}
	private void Course_NoPoisons()
	{
		CurrentGameState = GameState.RoundEnd;
	}
	private void Course_DrankNormal()
	{
		CurrentGameState = GameState.PreTurn;
	}
	private void Course_DrankPoison()
	{
		CurrentGameState = GameState.RoundEnd;
	}
	private CourseState CheckBottles()
	{
		var bottleLogic = GetComponent<BottleLogic>();
		CourseState targetState = CourseState.DrankNormal;
		if(DrankPoison) targetState = CourseState.DrankPoison;
		if(!bottleLogic.HasPoison()) targetState = CourseState.NoPoisons;
		if(bottleLogic.hasBottles()) targetState = CourseState.NoBottles;
		DrankPoison = false;
		return targetState;
	}
	/// ----------------------------------------
	/// ROUND LOGIC
	/// ----------------------------------------
	
	private TimeSince StateTimer;
	private void OnPreGame()
	{
		
		//Disable Mouse Input
		AssignPlayerID();
		PostProcessCon.Trigger(true);
		AttachPlayerCamera(0);

		if(StateTimer > 1) //Forced Delay
		{
			CurrentGameState = GameState.GameStart;
			StateTimer = 0;
		}
		
	}
	private void OnGameStart()
	{
		foreach (GameObject player in PlayerAsset) player.GetComponent<AnimManager>().anim_Sit(); //Sends Animation Callout. Refactor for cleaner animation execution.
		PostProcessCon.Trigger(false);
		
		if(StateTimer > 1) //Forced Delay
		{
			CurrentGameState = GameState.PreRound;
			StateTimer = 0;
		}
		
	}
	private void OnPreRound()
	{
		if(StateTimer > 1) //Forced Delay
		{
			CurrentGameState = GameState.RoundStart;
			StateTimer = 0;
		}
		
	}
	private void OnRoundStart()
	{
		var bottleLogic = GetComponent<BottleLogic>();
		bottleLogic.SpawnBottles();
		if(StateTimer > 1) //Forced Delay
		{
			CurrentGameState = GameState.PreTurn;
			StateTimer = 0;
		}
		
	}
	private void OnPreTurn()
	{
		CycleTurn();
		if(StateTimer > 1) //Forced Delay
		{
			CurrentGameState = GameState.TurnStart;
			StateTimer = 0;
		}
		
	}
	private void OnTurnStart()
	{
		//Enable Mouse Input
		Mouse.Visibility = MouseVisibility.Visible;
	}
	private void OnTurnHold()
	{
		//Don't do anything; To give room for player interaction.
		//We can add a turn timer here.
	}
	private void OnTurnEnd()
	{
		//Disable Mouse Input
		Mouse.Visibility = MouseVisibility.Hidden;
	}
	private void OnPostTurn()
	{
		if(StateTimer > 1) //Forced Delay
		{
			Course(CheckBottles());
			StateTimer = 0;
		}
		//Will go through each of the ending mechanics.
	}
	private void OnRoundEnd()
	{
		var bottleLogic = GetComponent<BottleLogic>();
		bottleLogic.KillBottles();
		if(StateTimer > 1) //Forced Delay
		{
			CurrentGameState = GameState.PostRound;
			StateTimer = 0;
		}
		
	}
	private void OnPostRound()
	{
		int deadman;
		if(isDead(out deadman)) 
		{
			if(StateTimer > 1) //Forced Delay
			{
				StateTimer = 0;
				CurrentGameState = GameState.GameEnd;
			}
			Log.Info("Player " + deadman + " has died. Game Over!"); //Temporary Callout to death.
		}
		else
		{
			if(StateTimer > 1) //Forced Delay
			{
				StateTimer = 0;
				CurrentGameState = GameState.RoundStart;
			}
		}
		
	}
	private void OnGameEnd()
	{
		CurrentGameState = GameState.PostGame;
	}
	private void OnPostGame()
	{

	}

	/// ----------------------------------------
	/// LOGIC SUBSYSTEMS
	/// ----------------------------------------
	
	
	
	/// ----------------------------------------
	/// PLAYER SYSTEMS
	/// ----------------------------------------
	
	[Property]
	public GameObject PlayerReference{ get; set; } //REFERENCE OBJECT FOR PLAYER INSTANTIATION
	[Property]
 	public GameObject[] PlayerSpawnPoints { get; set; } //SPAWNPOINTS FOR PLAYER INSTANCES
	private PlayerData[] Players = new PlayerData[2];	//PLAYER LIST FOR EASY REFERENCE
	private GameObject[] PlayerAsset = new GameObject[2]; //PLAYER GAMEOBJECT LIST FOR EASY MANIPULATION
	private int CurrentPlayer{get; set;} = 2;
	private int CycleTurn()
	{
		switch (CurrentPlayer)
		{
			case 0:
			return 1;
			case 1:
			return 2;
			case 2:
			return 0;
			default:
			return 0;
		}
	}

	private void InstancePlayerAsset(int PlayerInt) //SETS THE NECESSARY DETAILS FOR EACH PLAYER ALONG WITH THE SPAWNING OF A PLAYER.
	{
		PlayerAsset[PlayerInt] = PlayerReference.Clone();
		PlayerAsset[PlayerInt].Parent = PlayerSpawnPoints[PlayerInt];
		PlayerAsset[PlayerInt].LocalPosition = Vector3.Zero;
		PlayerAsset[PlayerInt].LocalRotation = Rotation.Identity;
		Players[PlayerInt] = new PlayerData(); 
		Players[PlayerInt].TeamState = Team.Player;
		Players[PlayerInt].Lives = 3;
		Players[PlayerInt].LivesMax = 3;
		Players[PlayerInt].DudsDrank = 0;
		Players[PlayerInt].PoisonsDrank = 0;
		Players[PlayerInt].PlayerInst = PlayerAsset[PlayerInt];
		
	}
	private void AssignPlayerID() //CHECKS EACH AVAILABLE SLOT FOR PLAYERS TO FILL. MUST ADD FALLBACK FOR FULL LOBBY.
	{
		if(PlayerAsset[0] == null) InstancePlayerAsset(0);
		if(PlayerAsset[1] == null) InstancePlayerAsset(1);
	}

	private void AttachPlayerCamera(int Player)
	{
		var _player = PlayerAsset[Player];
		var playerRelay = _player.GetComponent<PlayerRelay>();
		if(playerRelay.playerController != null)
		playerRelay.playerController.Enabled = true;
		var camera = Scene.Camera;
		camera.GameObject.Parent = playerRelay.EyeSlot;
		camera.GameObject.LocalPosition = Vector3.Zero;
		camera.GameObject.LocalRotation = Rotation.Identity;
	}
	public void PlayerDrink(bool isPoison, GameObject player, GameObject bottle) //WHEN A PLAYER DRINKS
	{
		PlayerData playerData = PlayerFind(player);
		Bottle _bottle = bottle.GetComponent<Bottle>();
		if( CanAct( player ) && _bottle.IsValid && !_bottle.empty )
		{
			if ( isPoison )
			{
				playerData.Lives--;
				playerData.PoisonsDrank++;
				DrankPoison = true;
			}
			else
			{
				playerData.DudsDrank++;
				DrankPoison = false;
			}
			bottle.Destroy();
		}
		
	}

	private PlayerData PlayerFind(GameObject player)
	{
		PlayerData playerData = null;
		for (int i = 0; i < Players.Count(); i++)
		{
			if(Players[i].PlayerInst == player)
			{
				playerData = Players[i];
			}
		}
		return playerData;
	}

	public bool CanAct(GameObject player)
	{
		for (int i = 0; i < Players.Length; i++)
		{	if (Players[i].PlayerInst == player && CurrentPlayer == i)
			{ return true; }
		}
		return false;
	}

	private bool isDead(out int player)
	{
		for (int i = 0; i < Players.Count(); i++)
		{
			if(Players[i].Lives <= 0)
			{
				player = i;
				return true;
			}
		}
		player = 2;
		return false;	
	}
	private bool isDead()
	{
		int player; //Throwaway
		return isDead(out player);
	}

	/// ----------------------------------------
	/// DEBUG SYSTEMS
	/// ----------------------------------------
	

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
	public int DrinksDrank{ get; set; }
	public int PoisonsDrank{ get; set; }
	public GameObject PlayerInst{ get; set; }
}
