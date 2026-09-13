using Sandbox;

public sealed class PlayerRelay : Component
{
	[Property]
	public GameObject PlayerCore{ get; set; }
	[Property]
	public SkinnedModelRenderer Renderer { get; set; }
	[Property]
	public GameObject EyeSlot { get; set; }
	[Property]
	public DD_PlayerController playerController { get; set; }
}
