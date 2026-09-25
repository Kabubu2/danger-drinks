using Sandbox;
using Sandbox.UI;

public sealed class ButtonStartGame : Clickable 
{
	
	[Property] SceneFile MainSceneBot {get;set;}

	public override void OnClick()
	{
		GetComponent<MultiplayerMove>()?.MultiMove();
	}
}
public sealed class MultiplayerMove : Component
{
	[Property] SceneFile MainScene {get;set;}
	public void MultiMove()
	{
	 if ( MainScene != null && Networking.IsHost)
	 {
	 var MultiScene = new SceneLoadOptions();
	 MultiScene.SetScene(MainScene);
	 Game.ChangeScene(MultiScene);
	 }
	 }
}