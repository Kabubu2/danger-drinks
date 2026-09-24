using Sandbox;
using Sandbox.UI;

public sealed class ButtonStartGame : Clickable
{
	[Property]private NetworkControl menu{get; set;}
	public override void OnClick()
	{
		menu.StartGame();
	}
}
