using Sandbox;
using Sandbox.UI;

public sealed class ButtonStartGame : Clickable 
{
	[Property]public GameObject menu{get; set;}
	public override void OnClick()
	{
		if(menu != null && menu.GetComponent<NetworkControl> != null)
	menu.GetComponent<NetworkControl> ().StartGame();
	}
}
