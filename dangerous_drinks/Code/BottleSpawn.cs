using System;
using Sandbox;


public sealed class BottleSpawn : Component
{


	[Property]
	private GameObject bottlePrefab;
	[Property]
	private GameObject bottleFolder;
	[Property]
	public float Spacing {get; set;} = 200f;
	[Property]
	private Vector3 Direction {get; set;} = Vector3.Forward;
	protected override void OnUpdate()
	{
		
		if (Input.Pressed("Attack1"))
		{
			int loopCount = new Random().Next(3, 7);

			for (int i = 0; i < loopCount; i++)
			{

				Vector3 localOffset = Direction.Normal * (i * Spacing);
			Transform spawnTransform = Transform.World.ToWorld( new Transform( localOffset ) );

			// Instantiate the prefab into the active scene
			GameObject instance = bottlePrefab.Clone( spawnTransform );
			
			
			instance.Parent = GameObject;


				//GameObject bottle = bottlePrefab.Clone();
				//bottle.Parent = bottleFolder;
				//bottle.LocalPosition = Vector3.Zero + (spawnOffset*i);
				
			}
			
		}
	}
}
