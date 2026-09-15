using System;
using Sandbox;

public enum PlayerState {start, idle, select, drink, poisoned, dud}
public sealed class AnimManager : Component
{
	///
	/// ANIMATION CONTROLLERS AND VARIABLES
	/// 
	[Property]
	private PlayerRelay PlayerRenderer {get; set;}
	[Property]
	private DD_PlayerController playerController{get; set;}

	public bool IsTurn {get; set;}
	public PlayerState playerState {get; set;}
	public GameObject bottleSelected {get; set;}
	protected override void OnFixedUpdate()
	{
		if(IsTurn)
		{
			switch (playerState)
				{
					case PlayerState.start:
					anim_Sit();
					break;
					case PlayerState.idle:
					break;
					case PlayerState.select:
					anim_Select(bottleSelected);
					break;
					case PlayerState.drink:
					anim_Drink();
					break;
					case PlayerState.dud:
					anim_DrinkNormal();
					break;
					case PlayerState.poisoned:
					anim_DrinkHurt();
					break;
			}
		}
		else
		{
			var a = PlayerRenderer.Renderer.Parameters;
			a.Set("dud", false);
			a.Set("drink", false);
			a.Set("poison", false);
			a.Set("select", false);
		}
	}

	[Property]
	public GameObject held{ get; set; }

	public void BottleState(bool value)
	{
		held.Enabled = value;
	}
	public void anim_Drink()	
	{
		PlayerRenderer.Renderer.Parameters.Set("drink", true);
	}
	public void anim_Sit()	
	{
		PlayerRenderer.Renderer.Parameters.Set("begin", true);
	}
	public void anim_DrinkHurt()	
	{
		PlayerRenderer.Renderer.Parameters.Set("poison", true);
	}
	public void anim_DrinkNormal()	
	{
		PlayerRenderer.Renderer.Parameters.Set("dud", true);
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
