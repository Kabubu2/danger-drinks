using Sandbox;

public sealed class PlayerHand2Target : Component
{
	[Property]
	public Vector3 targetPos;
	[Property]
	public Rotation targetRot;
	[Property]
	public GameObject targetPref;
	[Property]
	public bool EnableIKHand = false;
	protected override void OnUpdate()
	{
		if(targetPref != null)
		{
			
			GameObject target = targetPref.Children.FirstOrDefault( x => x.Name == "IK Target");

			if(target.IsValid) 
				{
			
					SkinnedModelRenderer renderer = GameObject.GetComponent<SkinnedModelRenderer>();
					Vector3 interp = Vector3.Lerp(renderer.Parameters.GetVector("ik.hand_left.position") ,target.WorldPosition, Time.Delta*5);
					renderer.Parameters.Set("ik.hand_left.enabled", EnableIKHand);
					renderer.Parameters.Set("ik.hand_left.position", interp);
					renderer.Parameters.Set("ik.hand_left.rotation", target.WorldRotation);

					
			
				}

		}
		
	}


}
