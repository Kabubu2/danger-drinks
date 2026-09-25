using Sandbox;
using Sandbox.UI;

public sealed class ButtonStartGame : Clickable 
{
	[Property] SceneFile MainScene {get;set;}
	[Property] SceneFile MainSceneBot {get;set;}

	public override void OnClick()
	{
	 if ( MainScene != null )
            {
                Scene.Load(MainScene);
            }
	}
}
