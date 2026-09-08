using Sandbox;
using System;
public sealed class PoisonLogic : Component

{
[Property]
private GameObject bottleContainer{get; set;}




	protected override void OnUpdate()
	{

		
		
if (Input.Pressed("Attack2"))
		{
			//Logic to randomly select 1-2 bottles from the bottleContainer and set their poisoned property to true

			int poisonCount = new Random().Next(1, 3);
			var availableBottles = bottleContainer.Children.ToList();
			var randomBottles = availableBottles.OrderBy(x => Guid.NewGuid()).Take(poisonCount);

			foreach (var filePath in randomBottles)
			{
				
				var bottle = filePath.Components.Get<Bottle>();
				bottle.poisoned = true;
			}
			
			
		}

	}
}

