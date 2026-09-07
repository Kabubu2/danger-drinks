using System;
using Sandbox;
using Sandbox.Diagnostics;


public sealed class BottleSpawn : Component
{


	[Property]
	private GameObject bottlePrefab {get; set;}
	[Property]
	private GameObject bottleFolder {get; set;}
	[Property]
	private Vector3 spawnOffset = new Vector3(0,0,0);	
	protected override void OnUpdate()
	{
		Spawn();
	}
	private void Spawn()
	{
		
		if (Input.Pressed("Attack1") && bottlePrefab != null)
		{
			var a = new Random().Next(3,6);

			for (int i = 0; i <= a; i++)
			{
				GameObject bottle = bottlePrefab.Clone();
				bottle.Parent = bottleFolder;
				bottle.LocalPosition = Vector3.Zero + (spawnOffset*i);
			}
		}

	}
}
