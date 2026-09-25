using Sandbox;

public sealed class KillMeParticle : Component
{
	[Property]ParticleSphereEmitter particleEffect{get;set;}
	public bool Instant {get; set;} = false;
	TimeSince timer;
	protected override void OnAwake()
	{
		timer = 0;
	}
	protected override void OnUpdate()
	{
		if (timer > 4 && !Instant)
		{
			particleEffect.Enabled = true;
			if(!particleEffect.IsValid) GameObject.Destroy();
		}
		if(Instant)
		{
			particleEffect.Enabled = true;
			if(!particleEffect.IsValid) GameObject.Destroy();
		}
		
	}
}
