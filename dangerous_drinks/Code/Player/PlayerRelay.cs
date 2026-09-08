using Sandbox;

public sealed class PlayerRelay : Component
{
	[Property]
	public GameObject EyeSlot{get; set;}
	[Property]
	public GameObject PlayerCore{get; set;}
	
	[Property]
	public SkinnedModelRenderer Renderer {get; set;}
}
