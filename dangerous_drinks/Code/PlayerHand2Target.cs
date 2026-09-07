using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
	[Property]
	private GameObject PlayerCore;
	[Property]
	private SkinnedModelRenderer Renderer;
	protected override void OnUpdate()
	{
		if(targetPref != null)
		{
			GameObject target = targetPref.Children.FirstOrDefault( x => x.Name == "IK Target");
			if(target.IsValid) 
				{
					Random a = new Random();
					var b = a.Int(3,7);
					Vector3 interpVec = Vector3.Lerp(Renderer.Parameters.GetVector("ik.hand_left.position") ,target.WorldPosition, Time.Delta*5);
					float interpBlend = MathX.Lerp(Renderer.Parameters.GetFloat("ik.lean.blend"),SpectrumStretch(Vector3.DistanceBetween(PlayerCore.WorldPosition, interpVec)),Time.Delta*5);
					Renderer.Parameters.Set("ik.hand_left.enabled", EnableIKHand);
					Renderer.Parameters.Set("ik.hand_left.position", interpVec);
					Renderer.Parameters.Set("ik.hand_left.rotation", target.WorldRotation);
					Renderer.Parameters.Set("ik.lean.position", interpVec);
					Renderer.Parameters.Set("ik.lean.blend", interpBlend);
							
				}
		}
	}
	
	// Minimum Distance Expected: 35
	// Maximum Distance Expected: 40
	private float SpectrumStretch(float distance)
	{
		var a = (distance-35f)/5f;
		a = Math.Clamp(a,0f,1f);
		return a;
	}

}
