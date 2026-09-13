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
		if(!hasBottles()) 
		{
			Bottles();
			AssignPoison();
		}
	}
	public bool HasPoison() //Does the container have poison?
	{
			if(hasBottles())
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
	public void KillBottles()
	{
		var bottles = bottleFolder.Children;
		if(hasBottles())
		{
			for (int i = 0; i < bottles.Count(); i++)
			{
				if(bottles[i].IsValid) bottles[i].Destroy();
			}
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
	public bool hasBottles()
	{
			if (!bottleFolder.IsValid)
			{
				Log.Error("Bottle Folder not found.");
				return false;
			}
			else
			{
				var a = bottleFolder.Children.Count() > 0 && bottleFolder.Children != null;
				return a;
			}
	}
	public string[] DebugInfo()
	{
		string[] debug = new string[2];

		debug[0] = "##ERR## Bottle Count";
		debug[1] = "##ERR## Poison Bottles";

		var a = bottleFolder.Children.Count();
		var b = 0;
		if(bottleFolder.Children != null && bottleFolder.Children.Count > 0)
		{
			for (int i = 0; i < bottleFolder.Children.Count; i++)
			{
				if(bottleFolder.Children[i].GetComponent<Bottle>() != null)
				{
					if(bottleFolder.Children[i].GetComponent<Bottle>().poisoned) b++;
				}
			}
			debug[0] = a.ToString() + " Bottle Count";
			debug[1] = b.ToString() + " Poison Bottles";
		}
		return debug;
	}
	private void AssignPoison()
	{
		int poisonCount = new Random().Next(1, 3); //Randomly select 1-2 bottles from the bottleContainer and set their poisoned property to true
		if(bottleFolder.IsValid & hasBottles())
		{
			var availableBottles = bottleFolder.Children.ToList();
			if(availableBottles != null)
			{
				var randomBottles = availableBottles.OrderBy(x => Guid.NewGuid()).Take(poisonCount);
				if(randomBottles.Count() > 0)
				{
					foreach (var bottle in randomBottles)
					{
						if(bottle.IsValid)
						{
							if(bottle.GetComponent<Bottle>() != null)
							{
								var _bottle = bottle.GetComponent<Bottle>();
								_bottle.poisoned = true;
							}
						}	
					}
				}
			}
			
		}
	}
}
