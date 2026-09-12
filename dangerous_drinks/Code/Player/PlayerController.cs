using System.Threading.Tasks;
using Sandbox;

public sealed class DD_PlayerController : Component
{
	private GameManager gameManager;
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
				var a = ClickZone("selectable");
				if (a != null)
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

	private void SendChoice(GameObject player, GameObject item)
	{
		gameManager.PlayerInteract(player, item);
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

			Vector2[] DirectionalOffset = new Vector2[9];
			{
				DirectionalOffset[0] = new Vector2(0,0);
				DirectionalOffset[1] = new Vector2(1,-1);
				DirectionalOffset[2] = new Vector2(-1,1);
				DirectionalOffset[3] = new Vector2(-1,-1);
				DirectionalOffset[4] = new Vector2(1,0);
				DirectionalOffset[5] = new Vector2(0,1);
				DirectionalOffset[6] = new Vector2(-1,0);
				DirectionalOffset[7] = new Vector2(0,-1);
				DirectionalOffset[8] = new Vector2(1,1);
			}

			for (int i = 0; i < 9; i++)
			{
				var ray = camera.ScreenPixelToRay(Mouse.Position + DirectionalOffset[i]);
				var trace = Scene.Trace.Ray( ray.Position, ray.Position + ray.Forward * 999999f).Run();
				if(!trace.Hit) continue;
				else if(trace.GameObject.Tags.Has(tag)) return trace.GameObject;
			}

		return null;
	}
	
}
