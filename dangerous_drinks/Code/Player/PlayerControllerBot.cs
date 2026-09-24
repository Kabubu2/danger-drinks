using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sandbox;


public sealed class DD_PlayerControllerBot : Component
{
	public bool bot{get; set;} = false;
	public GameManagerBot gameManager {get; set;}
	public int playerID {get; set;} = 0;
	
	private void CheckUp()
	{
		if ( !gameManager.IsValid )
		{
			gameManager = Scene.GetAll<GameManagerBot>().FirstOrDefault();
			Log.Info("Game Manager being searched for...");
		}
	}
	protected override void OnUpdate()
	{
		CheckUp();
		SelectFilter();
		
	}
	
	
	private void SelectFilter()
	{
		if(gameManager.IsValid)
		{
			if(Input.Pressed("Attack1")) 
			{
				int currentPlayer = gameManager.CurrentPlayer;
				var a = ClickZone("selectable");
				if (a != null && currentPlayer == 0)
				{	
					
					SendChoice(this.GameObject, a);
				}
			}
		}
	}

	protected override void OnAwake()
	{
		gameManager = Scene.GetAll<GameManagerBot>().FirstOrDefault();
	}

	private void SendChoice(GameObject player, GameObject item)
	{
		//gameManager.PlayerDrink(player, item);
		if(player != null && item != null && gameManager.CurrentPlayer == 0)
		{
			if(item.GetComponent<Bottle>() != null)
			{
				gameManager.SelectDrink(item.GetComponent<Bottle>().poisoned, player, item);
			}
		}
	}
	
	private GameObject ClickZone()
	{
		Log.Info("ClickZone");
		var camera = Scene.Camera;
		if(!camera.IsValid) return null;
		
		var ray = camera.ScreenPixelToRay(Mouse.Position);
		var trace = Scene.Trace.Ray( ray.Position, ray.Position + ray.Forward * 999999f).Run();

		if(trace.GameObject.IsValid) return trace.GameObject;
		return null;
	}
	private GameObject ClickZone(string tag)
	{

		var camera = Scene.Camera;
		if(!camera.IsValid) return null;

		var ray = camera.ScreenPixelToRay(Mouse.Position);
		var trace = Scene.Trace.Ray( ray.Position, ray.Position + ray.Forward * 999999f).Run();
		if(trace.Hit)Log.Info(trace.GameObject.Tags);
		if(trace.GameObject.Tags.Has(tag)) return trace.GameObject;
			

		return null;
	}
	
}
