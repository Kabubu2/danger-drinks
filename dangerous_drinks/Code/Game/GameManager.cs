using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Network;
using Sandbox.UI;
using Sandbox.Utility;

public enum GameState {None, PreGame, GameStart, PreRound, RoundStart, PreTurn, TurnStart, TurnHold, TurnEnd, PostTurn, RoundEnd, PostRound, GameEnd, PostGame}
public enum CourseState {None, NoBottles, NoPoisons, DrankPoison, DrankNormal}
public sealed class GameManager : Component, Component.INetworkListener
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
	public GameObject bottleSelected{get; set;}

	private bool isHost {get; set;}
	protected override void OnUpdate()
	{
		if(!Start)StartGame();
		Logic();
	}
	private bool Start {get; set;} = false;
	private void StartGame()
	{
		isHost = Networking.IsHost;
		Start = true;
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
	[Sync] public GameState CurrentGameState{ get; set; } //The current state of the game.

	/// ----------------------------------------
	/// COURSE LOGIC
	/// ----------------------------------------

	public bool DrankPoison;
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
		var bottleLogic = GetComponent<BottleLogic>();
		if(!bottleLogic.HasPoison())
		CurrentGameState = GameState.RoundEnd;
	}
	private CourseState CheckBottles()
	{
		var bottleLogic = GetComponent<BottleLogic>();
		CourseState targetState = CourseState.DrankNormal;
		if(DrankPoison) targetState = CourseState.DrankPoison;
		if(!bottleLogic.HasPoison()) targetState = CourseState.NoPoisons;
		if(!bottleLogic.hasBottles()) targetState = CourseState.NoBottles;
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
		Input.Suppressed = true;
		PostProcessCon.Trigger(true);
		if (isHost)  AttachPlayerCamera(0);
		else AttachPlayerCamera(0);
		
		

		if(StateTimer > 2) //Forced Delay
		{
			CurrentGameState = GameState.GameStart;
			StateTimer = 0;
		}
		
	}
	private void OnGameStart()
	{
		
		foreach (GameObject player in PlayerAsset) 
		
		if (player.IsValid)
		{
		player.GetComponent<AnimManager>().anim_Sit(); //Sends Animation Callout. Refactor for cleaner animation execution.
		PostProcessCon.Trigger(false);
		}
		
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
		if (Networking.IsClient) return;
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
		if(StateTimer > 1) //Forced Delay
		{
			CurrentPlayer = CycleTurn();
			CurrentGameState = GameState.TurnStart;
			StateTimer = 0;
		}
		
	}
	private void OnTurnStart()
	{
		//Enable Mouse Input
		
		Mouse.Visibility = MouseVisibility.Visible;
		if(StateTimer > 1) //Forced Delay
		{
			CurrentGameState = GameState.TurnHold;
			StateTimer = 0;
		}
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
		if(StateTimer > 1) //Forced Delay
		{
			CurrentGameState = GameState.PostTurn;
			StateTimer = 0;
		}
	}
	private void OnPostTurn()
	{
		if(StateTimer > 3) //Forced Delay
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
	[Sync]public GameObject PlayerReference{ get; set; } //REFERENCE OBJECT FOR PLAYER INSTANTIATION
	[Property]
 	[Sync]public GameObject[] PlayerSpawnPoints { get; set; } //SPAWNPOINTS FOR PLAYER INSTANCES
	[Sync]private PlayerData[] Players {get; set;} = new PlayerData[2];	//PLAYER LIST FOR EASY REFERENCE
	[Sync]public GameObject[] PlayerAsset {get; set;} = new GameObject[2]; //PLAYER GAMEOBJECT LIST FOR EASY MANIPULATION
	[Sync]public int CurrentPlayer{get; set;} = 2;
	private int CycleTurn()
	{
		switch (CurrentPlayer)
		{
			case 0:
			return 1;
			case 1:
			return 0;
			case 2:
			return 0;
			default:
			return 0;
		}
	}
	
	void INetworkListener.OnActive( Connection connection )
	{
		Log.Info( $"{connection.DisplayName} has become active" );
		if (isHost && Players[1] == null) InstancePlayerAsset(1, connection);
		else if (!isHost && Players[0] == null) InstancePlayerAsset(0, connection);
		
		if ( Players.Count() != 2 ) return;
		for (int i = 0; i < Players.Count(); i++)
		{
			if(Players[i] == null || Players[i].PlayerInst == null) return;
		}

		CurrentGameState = GameState.PreGame;
		
		
	}
	
		private void InstancePlayerAsset(int PlayerInt, Connection connection) //SETS THE NECESSARY DETAILS FOR EACH PLAYER ALONG WITH THE SPAWNING OF A PLAYER.
	{
		PlayerAsset[PlayerInt] = PlayerReference.Clone();
		PlayerAsset[PlayerInt].Parent = PlayerSpawnPoints[PlayerInt];
		PlayerAsset[PlayerInt].LocalPosition = Vector3.Zero;
		PlayerAsset[PlayerInt].LocalRotation = Rotation.Identity;
		PlayerAsset[PlayerInt].NetworkSpawn(connection);
		PlayerAsset[PlayerInt].GetComponent<DD_PlayerController>().playerID = PlayerInt;
		Players[PlayerInt] = new PlayerData
		{	
			TeamState = Team.Player,
			Lives = 3,
			LivesMax = 3,
			DudsDrank = 0,
			PoisonsDrank = 0,
			PlayerInst = PlayerAsset[PlayerInt]
		
		};
			Players[PlayerInt].UserID = new UserData
			{
				Steamid = connection.SteamId,
				
			};
		
		if (Players[PlayerInt] != null && Players[PlayerInt].PlayerInst != null)
		{
			if((isHost && PlayerInt == 0) ||(!isHost && PlayerInt == 1))
			{
				var a = Players[PlayerInt].PlayerInst.GetComponent<PlayerRelay>();
				a.playerController.Enabled = true;
				AttachPlayerCamera(PlayerInt);
			} 
		}
		
	}

	

	[Rpc.Broadcast]
	private void AttachPlayerCamera(int Player)
	{
		if(Networking.IsHost){
		var _player = PlayerAsset[0];
		var playerRelay = _player.GetComponent<PlayerRelay>();
		if(playerRelay.playerController != null)
		playerRelay.playerController.Enabled = true;
		var camera = Scene.Camera;
		camera.GameObject.Parent = playerRelay.EyeSlot;
		camera.GameObject.LocalPosition = Vector3.Zero;
		camera.GameObject.LocalRotation = Rotation.Identity;
		PlayerAsset[1].GetComponent<DD_PlayerController>().Enabled = false;
		}
		
		if(Networking.IsClient){
		if (PlayerAsset == null || PlayerAsset.Length <= 1 || PlayerAsset[1] == null) return;
    	var _player = PlayerAsset[1];
		var playerRelay = _player.GetComponent<PlayerRelay>();
		if(playerRelay.playerController != null)
		playerRelay.playerController.Enabled = true;
		var camera = Scene.Camera;
		camera.GameObject.Parent = playerRelay.EyeSlot;
		camera.GameObject.LocalPosition = Vector3.Zero;
		camera.GameObject.LocalRotation = Rotation.Identity;
		PlayerAsset[0].GetComponent<DD_PlayerController>().Enabled = false;
		}
	}

	public void SelectDrink(bool isPoison, GameObject player, GameObject bottle)
	{
		PlayerData playerData = PlayerFind(player);
		Bottle _bottle = bottle.GetComponent<Bottle>();
		if( CanAct( player ) && _bottle.IsValid )
		{
			if(bottle != bottleSelected) bottleSelected = bottle;
			else PlayerDrink(isPoison, player, bottle);
		}
		
	}
	public void PlayerDrink(bool isPoison, GameObject player, GameObject bottle) //WHEN A PLAYER DRINKS
	{
		PlayerData playerData = PlayerFind(player);
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
		bottleSelected = null;
		bottle.Destroy();
		CurrentGameState = GameState.TurnEnd;
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
}

/// ----------------------------------------
/// PLAYER UNIVERSAL DATA AND VARIABLES
/// ----------------------------------------

public enum Team { Player, Opponant, Neither }
public class UserData
{
	//SteamID
	public ulong Steamid { get; set; }
	//PlayerName
	public string Steamname { get; set; }
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
