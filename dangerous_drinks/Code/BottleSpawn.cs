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
			int loopCount = System.Random.Shared.Next (3,7);

			for (int i = 0; i < loopCount; i++)
			{
				bottlePrefab.Clone(spawnOrigin.WorldPosition);
			}
			
		}
	}
}
