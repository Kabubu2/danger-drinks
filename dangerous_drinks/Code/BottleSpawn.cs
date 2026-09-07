using Sandbox;

public sealed class BottleSpawn : Component
{
	
	protected override void OnUpdate()
	{
		if (Input.Pressed("Attack1"))
		{
			GameObject.GetPrefab("Bottle").Clone(Vector3.Random * 100);
		}
	}
}
