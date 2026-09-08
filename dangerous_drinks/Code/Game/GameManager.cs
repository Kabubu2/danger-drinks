using Sandbox;

public sealed class GameManager : Component
{
	private GameObject playerPrefab {get; set;}
	protected override void OnUpdate()
	{

	}
	private PlayerData[] Players = new PlayerData[2];
	[Property]
	private GameObject PlayerRef{ get; set; }
	private GameObject[] PlayerSpawnPoints { get; set; }
	private void OnRoundStart()
	{
		//Player
		Players[0].TeamState = Team.Player;
		Players[0].Lives = 3;
		Players[0].LivesMax = 3;
		Players[0].DudsDrank = 0;
		Players[0].PoisonsDrank = 0;


		//Opponant
		Players[1].TeamState = Team.Player;
		Players[1].Lives = 3;
		Players[1].LivesMax = 3;
		Players[1].DudsDrank = 0;
		Players[1].PoisonsDrank = 0;
	}
	private void InstancePlayerAsset(int PlayerInt)
	{
		
	}
}

public enum Team { Player, Opponant, Neither }
public class PlayerData
{
	private PlayerData()
	{
	}
	public Team TeamState { get; set; } = Team.Neither;
	public int Lives{ get; set; } = 3;
	public int LivesMax{ get; set; } = 3;
	public int DudsDrank{ get; set; } = 0;
	public int PoisonsDrank{ get; set; } = 0;
	public GameObject PlayerInst{ get; set; } = null;
}
