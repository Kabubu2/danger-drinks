using System;
using Sandbox;


public sealed class BottleSpawn : Component
{


	[Property]
	private GameObject bottlePrefab;
	[Property]
	private GameObject bottleFolder;	
	protected override void OnUpdate()
	{
		
		if (Input.Pressed("Attack1"))
		{
			int loopCount = new Random().Next(1, 7);

			for (int i = 0; i < loopCount; i++)
			{
				bottlePrefab.Clone( bottleFolder.WorldPosition);
			}
			
		}
	}
}
