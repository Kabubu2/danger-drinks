using System;
using Sandbox;

public enum GameState {PreGame, GameStart, RoundStart, RoundEnd, GameEnd}
public sealed class GameManager : Component
{
	/// <summary>
	/// GAME LOGIC AND VARIABLES
	/// </summary>
	private GameObject WaitPoint;
	protected override void OnUpdate()
	{

	}
	private GameState CurrentGameState;



	/// ----------------------------------------
	/// ROUND LOGIC
	/// ----------------------------------------

	private void OnPreGame() //FIRES BEFORE ANYONE CAN SEE ANYTHING OR DO ANYTHING.
	{
		AssignJoinID();
	}
	private void OnGameStart() //FIRES WHEN PREGAME IS DONE AND THE PLAYERS ARE SUCCESSFULLY PLACED.
	{
		
	}
	private void OnRoundStart() //FIRES WHEN GAMESTART IS OVER AND ROUND HAS BEGUN.
	{
		
		//Player 1 - Host
		//Player 2 - Opponant
	}


	/// ----------------------------------------
	/// PLAYER SYSTEMS
	/// ----------------------------------------

	[Property]
	private GameObject PlayerRef{ get; set; } //REFERENCE OBJECT FOR PLAYER INSTANTIATION
	[Property]
 	private GameObject[] PlayerSpawnPoints { get; set; } //SPAWNPOINTS FOR PLAYER INSTANCES
	private PlayerData[] Players = new PlayerData[2];	//PLAYER LIST FOR EASY REFERENCE
	private GameObject[] PlayerAsset = new GameObject[2]; //PLAYER GAMEOBJECT LIST FOR EASY MANIPULATION

	private void AssignJoinID() //CHECKS EACH AVAILABLE SLOT FOR PLAYERS TO FILL. MUST ADD FALLBACK FOR FULL LOBBY.
	{
		for (int i = 0; i < PlayerAsset.Length; i++)
		{
			if(PlayerAsset[i] == null)
			{
				InstancePlayerAsset(i);
			}
			else continue;
		}

	}
	private void InstancePlayerAsset(int PlayerInt) //SETS THE NECESSARY DETAILS FOR EACH PLAYER ALONG WITH THE SPAWNING OF A PLAYER.
	{
		Players[PlayerInt].TeamState = Team.Player;
		Players[PlayerInt].Lives = 3;
		Players[PlayerInt].LivesMax = 3;
		Players[PlayerInt].DudsDrank = 0;
		Players[PlayerInt].PoisonsDrank = 0;
		PlayerAsset[PlayerInt] = PlayerRef.Clone();
		PlayerAsset[PlayerInt].Parent = PlayerSpawnPoints[1];
		PlayerAsset[PlayerInt].LocalPosition = Vector3.Zero;
		PlayerAsset[PlayerInt].LocalRotation = Rotation.Identity;
	}
	private void AttachCamera(int Player)
	{
		
	}


	///
	/// ANIMATION CONTROLLERS AND VARIABLES
	/// 
	
	private float animVar_Stretch(float distance)
	{
		var a = (distance-35f)/5f;
		a = Math.Clamp(a,0f,1f);
		return a;
	}

   	private void anim_Select(GameObject anim_bottle_target, int Player)
	{
		PlayerRelay PlayerRenderer = PlayerAsset[Player].GetComponent<PlayerRelay>();
		if(anim_bottle_target.IsValid)
		{
			var target = anim_bottle_target.Parent.Children.FirstOrDefault( x => x.Name == "IK Target");
			if (target.IsValid)
			{
					Vector3 interpVec = Vector3.Lerp(PlayerRenderer.Renderer.Parameters.GetVector("ik.hand_left.position") ,target.WorldPosition, Time.Delta*5);
					float interpBlend = MathX.Lerp(PlayerRenderer.Renderer.Parameters.GetFloat("ik.lean.blend"),animVar_Stretch(Vector3.DistanceBetween(PlayerRenderer.PlayerCore.WorldPosition, interpVec)), Time.Delta * 5);
					PlayerRenderer.Renderer.Parameters.Set("ik.hand_left.enabled", true);
					PlayerRenderer.Renderer.Parameters.Set("ik.hand_left.position", interpVec);
					PlayerRenderer.Renderer.Parameters.Set("ik.hand_left.rotation", target.WorldRotation);
					PlayerRenderer.Renderer.Parameters.Set("ik.lean.position", interpVec);
					PlayerRenderer.Renderer.Parameters.Set("ik.lean.blend", interpBlend);
			}
		}
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
	private PlayerData()
	{
	}
	public UserData UserID {get; set;}
	public Team TeamState { get; set; } = Team.Neither;
	public int Lives{ get; set; } = 3;
	public int LivesMax{ get; set; } = 3;
	public int DudsDrank{ get; set; } = 0;
	public int PoisonsDrank{ get; set; } = 0;
	public GameObject PlayerInst{ get; set; } = null;
}
