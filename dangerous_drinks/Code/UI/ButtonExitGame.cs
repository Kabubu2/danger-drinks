using Sandbox;

public sealed class ButtonExitGame : Clickable
{
	public override void OnClick()
	{
		Game.Close();
	}
}
