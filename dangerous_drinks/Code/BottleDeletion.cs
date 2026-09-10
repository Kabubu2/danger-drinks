using Sandbox;

public sealed class BottleDeletion : Component


{
	[Property]
	GameObject BottleFile { get; set; }

	[Property]
	public GameObject targetPref;
	protected override void OnUpdate()
	{
if (Input.Pressed("Reload"))
		{
			//All bottles deleted
			foreach (var filePath in BottleFile.Children)
			{
				if (BottleFile.Children.Count == 0)
				{
					return;
				}
				filePath.Destroy();
			}

		}

		if (Input.Pressed("Delete"))
		{
			//Delete a selected bottle
			GameObject targetPrefab = targetPref;
			targetPrefab.Destroy();
		}
	}
}
