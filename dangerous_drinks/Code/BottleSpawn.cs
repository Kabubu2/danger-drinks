using System;
using Sandbox;
using Sandbox.Diagnostics;


public sealed class BottleSpawn : Component
{


	[Property]
	private GameObject bottlePrefab {get; set;}
	[Property]
	private GameObject bottleFolder;
	[Property]
	private Vector3 spawnOffset = new Vector3(0,0,0);	
	protected override void OnUpdate()
	{
		
		if (Input.Pressed("Attack1"))
		{
			var a = new Random().Next(3,6);

			for (int i = 0; i <= a; i++)
			{

				GameObject bottle = bottlePrefab.Clone(bottleFolder, Vector3.Zero, Rotation.Identity, Vector3.One);
				bottle.Parent = bottleFolder;
				bottle.LocalPosition = Vector3.Zero + (spawnOffset*i);
				
			}
			
		}
		
	}
}
