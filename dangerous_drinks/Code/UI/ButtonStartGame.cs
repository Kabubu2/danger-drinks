using System.Diagnostics;
using Sandbox;
using Sandbox.UI;

public sealed class ButtonStartGame : Clickable 
{
	
	[Property] SceneFile MainScene {get;set;}
	[Property] SceneFile MainSceneBot {get;set;}

	public override void OnClick()
	{
		var a = Connection.All.Count();
		if(a == 1) GetComponent<MultiplayerMove>()?.MultiMove(MainSceneBot);
		if(a == 2) GetComponent<MultiplayerMove>()?.MultiMove(MainScene);
		
	}
}
public sealed class MultiplayerMove : Component
{
	public void MultiMove(SceneFile scene)
	{
	 if ( scene != null && Networking.IsHost)
	 {
	 	var MultiScene = new SceneLoadOptions();
	 	MultiScene.SetScene(scene);
	 	Game.ChangeScene(MultiScene);
	 }
	}
}

	 
