using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sandbox;


public sealed class DD_PlayerController : Component
{
	public GameManager gameManager {get; set;}
	public int playerID {get; set;} = 2;
	
	private void CheckUp()
	{
		if ( !gameManager.IsValid )
		{
			gameManager = Scene.GetAll<GameManager>().FirstOrDefault();
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
				if (a != null && currentPlayer < 2)
				{	
					SendChoice(this.GameObject, a);
				}
			}
		}
	}

	protected override void OnAwake()
	{
		gameManager = Scene.GetAll<GameManager>().FirstOrDefault();
	}

	[Rpc.Broadcast]
	private void SendChoice(GameObject player, GameObject item)
	{
		//gameManager.PlayerDrink(player, item);
		if(player.IsValid && item.IsValid && gameManager.CurrentPlayer == playerID)
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
		Log.Info("Trace");
			
		return null;
	}
	private GameObject ClickZone(string tag)
	{

		var camera = Scene.Camera;
		if(!camera.IsValid) return null;

		var ray = camera.ScreenPixelToRay(Mouse.Position);
		var trace = Scene.Trace.Ray( ray.Position, ray.Position + ray.Forward * 999999f).Run();

		if(trace.GameObject.Tags.Has(tag)) return trace.GameObject;
			

		return null;
	}
	
}
