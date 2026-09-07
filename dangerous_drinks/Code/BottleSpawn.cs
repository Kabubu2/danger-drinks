using Sandbox;

public sealed class BottleSpawn : Component
{
	[Property]
	private GameObject bottlePrefab;
	[Property]
	private GameObject spawnOrigin;
	protected override void OnUpdate()
	{
		if (Input.Pressed("Attack1"))
		{
			bottlePrefab.Clone(spawnOrigin.WorldPosition);
		}
	}
}
