using Sandbox;
using Sandbox.Rendering;

public sealed class PostProcessingDarkFade : BasePostProcess<PostProcessingDarkFade>
{
	[Property, Range( 0, 1 )]
	public float Transition{ get; set; }

	public override void Render()
	{
		if ( Transition.AlmostEqual( 0.0f ) ) return;

		Attributes.Set( "transition", Transition );

		var shader = Material.FromShader( "shaders/postprocess/fade.shader" );
		var blit = BlitMode.WithBackbuffer( shader, Stage.AfterPostProcess, 200, true );
		Blit( blit, "Transition" );
	}
}
