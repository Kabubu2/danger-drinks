using Sandbox;

public sealed class KillMe : Component
{
	[Property]public GameObject Connected{get;set;}
	public ParticleEffect particleEffect{get;set;}
	private bool _hasStarted;
	bool _hasParticle;
	protected override void OnUpdate()
	{
		if(particleEffect == null) particleEffect = Connected.GetComponent<ParticleEffect>();
		if(!_hasParticle && Connected != null && particleEffect != null)
		{
			particleEffect.OnParticleCreated += HasStarted;
			_hasParticle = true;
		}
	}
	public void HasStarted(Particle particle)
	{
		
		if(_hasStarted) return;
		_hasStarted = true;
		DeleteMe();
	}
	public void DeleteMe()
	{
		this.GameObject.Destroy();
	}
}
