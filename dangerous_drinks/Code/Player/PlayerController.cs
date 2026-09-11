using Sandbox;

public sealed class DD_PlayerController : Component
{

	protected override void OnUpdate()
	{
		//if(	Input.Pressed("Attack 1") ) ClickZone("selectable").Destroy();
		if(	Input.Pressed("Attack1") ) Log.Info(ClickZone().Name);
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
