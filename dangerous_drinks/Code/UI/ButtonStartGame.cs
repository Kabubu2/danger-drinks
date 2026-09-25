using Sandbox;
using Sandbox.UI;

public sealed class ButtonStartGame : Clickable 
{
	
	[Property] SceneFile MainSceneBot {get;set;}

	public override void OnClick()
	{
		
		
		
	}
public class MultiplayerScene : Component
	{
		[Property] SceneFile MainScene {get;set;}
		public void MultiSceneChange()
		{
			if(!Networking.IsHost) return;
			var MultiScene = new SceneLoadOptions();
			MultiScene.SetScene(MainScene);
			Game.ChangeScene(MultiScene);
		}
	}
}
