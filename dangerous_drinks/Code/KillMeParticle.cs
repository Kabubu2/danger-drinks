using Sandbox;

public sealed class KillMeParticle : Component
{
	[Property]ParticleSphereEmitter particleEffect{get;set;}
	TimeSince timer;
	protected override void OnAwake()
	{
		timer = 0;
	}
	protected override void OnUpdate()
	{
		if (timer > 4)
		{
			particleEffect.Enabled = true;
			if(!particleEffect.IsValid) GameObject.Destroy();
		}
		
	}
}
