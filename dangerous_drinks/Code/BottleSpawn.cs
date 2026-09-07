using System;
using Sandbox;


public sealed class BottleSpawn : Component
{


	[Property]
	private GameObject bottlePrefab;
	[Property]
	private GameObject bottleFolder;
	[Property]
	private Vector3 spawnOffset = new Vector3(0,0,0);	
	protected override void OnUpdate()
	{
		
		if (Input.Pressed("Attack1"))
		{
			int loopCount = new Random().Next(1, 7);

			for (int i = 0; i < loopCount; i++)
			{
				GameObject bottle = bottlePrefab.Clone();
				bottle.Parent = bottleFolder;
				bottle.LocalPosition = Vector3.Zero + (spawnOffset*i);
				
			}
			
		}
	}
}
