using Sandbox;

public sealed class PlayerHand2Target : Component
{
	[Property]
	public Vector3 targetPos;
	[Property]
	public Rotation targetRot;
	[Property]
	public GameObject prefab;
	[Property]
	public bool EnableIKHand = false;
	protected override void OnUpdate()
	{
		
		GameObject target = prefab.Children.FirstOrDefault( x => x.Name == "IK Target");

		if(target.IsValid) 
		{
			
			SkinnedModelRenderer renderer = GameObject.GetComponent<SkinnedModelRenderer>();
			renderer.Parameters.Set("ik.hand_left.enabled", EnableIKHand);
			renderer.Parameters.Set("ik.hand_left.position", target.WorldPosition);
			renderer.Parameters.Set("ik.hand_left.rotation", target.WorldRotation);
			
		}
	}


}
