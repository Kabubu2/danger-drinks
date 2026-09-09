using System;
using Sandbox;

public sealed class PostProcessController : Component
{
	[Property]
	public PostProcessingDarkFade Fade;
	public float wait = 4f;
	private float blend = 0f;
	private bool state = false;

	protected override void OnUpdate()
	{
		
		if(StateTimer >= 0f)
		{
			if(state)
			{
				blend = MathX.Lerp( blend, 1, Time.Delta * 10 );
			}
			else
			{
				blend = MathX.Lerp( blend, 0, Time.Delta * 10 );
			}
			Fade.Transition = blend;
		}

	}
	
	public void Trigger()
	{
		state = !state;
		StateTimer = wait;
	}
	public void Trigger(bool _state)
	{
		state = _state;
		StateTimer = wait;
	}
	private TimeUntil StateTimer = 4f;
}
