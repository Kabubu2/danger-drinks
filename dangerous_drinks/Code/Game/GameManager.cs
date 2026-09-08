using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Sandbox;

public enum GameState {PreGame, GameStart, RoundStart, RoundEnd, GameEnd, Error}
public sealed class GameManager : Component
{
	/// <summary>
	/// GAME LOGIC AND VARIABLES
	/// </summary>
	[Property]
	private GameObject WaitPoint;
	[Property]
	private GameObject Camera;
	
	private TimeUntil stateTimer = 2f;
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
			case GameState.RoundStart:
			OnRoundStart();
			break;
			case GameState.RoundEnd:
			break;
			case GameState.GameEnd:
			break;
			case GameState.Error:
			break;
			default:
			break;
		}
	}
	[Property]
	private GameState CurrentGameState{get; set;}

	/// ----------------------------------------
	/// ROUND LOGIC
	/// ----------------------------------------
	private void OnPreGame() //FIRES BEFORE ANYONE CAN SEE ANYTHING OR DO ANYTHING.
	{
		//Preloading
		AssignJoinID();

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
		//Transition to RoundStart
		CurrentGameState = GameState.RoundStart;
	}
	private void OnRoundStart() //FIRES WHEN GAMESTART IS OVER AND ROUND HAS BEGUN.
	{
		
		//Player 1 - Host
		//Player 2 - Opponent
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
		
	}
	private void AttachCamera(int Player)
	{
		var playerRelay = PlayerAsset[Player].GetComponent<PlayerRelay>();
		Camera.Parent = playerRelay.EyeSlot;
		Camera.LocalPosition = Vector3.Zero;
		Camera.LocalRotation = Rotation.Identity;
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
