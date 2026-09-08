using System;
using Sandbox;

public sealed class AnimManager : Component
{
	
	///
	/// ANIMATION CONTROLLERS AND VARIABLES
	/// 
	[Property]
	public PlayerRelay PlayerRenderer {get; set;}

	public void anim_Drink()	
	{
		PlayerRenderer.Renderer.Parameters.Set("drink", true);
	}

	public void anim_Sit()	
	{
		PlayerRenderer.Renderer.Parameters.Set("begin", true);
	}

	public float animVar_Stretch(float distance)
	{
		var a = (distance-35f)/5f;
		a = Math.Clamp(a,0f,1f);
		return a;
	}

   	public void anim_Select(GameObject anim_bottle_target)
	{
		if(anim_bottle_target.IsValid)
		{
			var target = anim_bottle_target.Parent.Children.FirstOrDefault( x => x.Name == "IK Target");
			if (target.IsValid)
			{
				PlayerRenderer.Renderer.Parameters.Set("select", true);
				Vector3 interpVec = Vector3.Lerp(PlayerRenderer.Renderer.Parameters.GetVector("ik.hand_left.position") ,target.WorldPosition, Time.Delta*5);
				float interpBlend = MathX.Lerp(PlayerRenderer.Renderer.Parameters.GetFloat("ik.lean.blend"),animVar_Stretch(Vector3.DistanceBetween(PlayerRenderer.PlayerCore.WorldPosition, interpVec)), Time.Delta * 5);
				PlayerRenderer.Renderer.Parameters.Set("ik.hand_left.enabled", true);
				PlayerRenderer.Renderer.Parameters.Set("ik.hand_left.position", interpVec);
				PlayerRenderer.Renderer.Parameters.Set("ik.hand_left.rotation", target.WorldRotation);
				PlayerRenderer.Renderer.Parameters.Set("ik.lean.position", interpVec);
				PlayerRenderer.Renderer.Parameters.Set("ik.lean.blend", interpBlend);
			}
		}
	}
}
