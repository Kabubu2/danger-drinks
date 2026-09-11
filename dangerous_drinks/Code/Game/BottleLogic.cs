using System;
using System.Diagnostics;
using Sandbox;


public sealed class BottleLogic : Component
{

	[Property]
	private GameObject bottlePrefab;
	[Property]
	private GameObject bottleFolder;
	[Property]
	private Vector3 offset = new Vector3 (5,0,0);
	
	public void SpawnBottles()
	{
		if(!hasBottles)
		{
			Bottles();
			AssignPoison();
		}
		
		else Log.Warning("Oops! All bottles.");
	}
	public bool HasPoison //Does the container have poison?
	{
		get
		{
			if(hasBottles)
			{
				var _folder = bottleFolder.Children;
				for (int i = 0; i < _folder.Count; i++)
				{
					var a = _folder[i].GetComponent<Bottle>();
					if( a.IsValid && a.poisoned ) return true;
					else continue;
				}
			}
			return false;
		}
	}
	public void KillBottles()
	{
		var hasBottles = bottleFolder.Children.Count > 0;
		foreach (GameObject bottle in bottleFolder.Children)
		{
			bottle.Destroy();
		}
	}

	private void Bottles()
	{
		int loopCount = new Random().Next(3, 6);

			for (int i = 0; i <= loopCount; i++)
			{
				//Bottle Offset relative to origin.
				Vector3 relativeOffset = ((bottleFolder.WorldPosition - ((offset * loopCount)/2)) + (offset * i));

				// Instantiate the prefab into the bottle folder.
				GameObject bottle = bottlePrefab.Clone();
				bottle.Parent = bottleFolder;
				bottle.WorldPosition = relativeOffset;
				bottle.Name = new string("Bottle " + i);
			}

	}
	private bool hasBottles
	{
		get
		{
			return bottleFolder.Children.Count > 0;
		}
	}
	private void AssignPoison()
	{
			//Randomly select 1-2 bottles from the bottleContainer and set their poisoned property to true
			int poisonCount = new Random().Next(1, 3);
			var availableBottles = bottleFolder.Children.ToList();
			var randomBottles = availableBottles.OrderBy(x => Guid.NewGuid()).Take(poisonCount);

			foreach (var bottle in randomBottles)
			{
				var _bottle = bottle.Components.Get<Bottle>();
				_bottle.poisoned = true;
			}
	}
}
