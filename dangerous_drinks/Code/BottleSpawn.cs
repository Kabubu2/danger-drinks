using System;
using Sandbox;


public sealed class BottleSpawn : Component
{


	[Property]
	private GameObject bottlePrefab;
	
	[Property]
	private GameObject bottleFolder;

	[Property]
	private Vector3 spawnOffset { get; set; }

	protected override void OnUpdate()
	{
		
		if (Input.Pressed("Attack1"))
		{
			//Randomize the number of bottles spawned between 3 and 6
			int loopCount = new Random().Next(3, 7);

			//Spawn bottles in a line with the specified offset
			for (int i = 0; i < loopCount; i++)
			
			{
			//Dynamic bottle spawn offset based on the number of bottles spawned and puts them in bottle folder
				GameObject bottle = bottlePrefab.Clone();
				bottle.Parent = bottleFolder;
				var dynamicOrigin = (spawnOffset * loopCount) / 2;
				bottle.LocalPosition = -dynamicOrigin + (spawnOffset*i);
				
			}
			
		}
	}
}
