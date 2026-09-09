
HEADER
{
	Description = "";
}

FEATURES
{
	#include "common/features.hlsl"
}

MODES
{
	Forward();
	Depth();
	ToolsShadingComplexity( "tools_shading_complexity.shader" );
}

COMMON
{
	#ifndef S_ALPHA_TEST
	#define S_ALPHA_TEST 0
	#endif
	#ifndef S_TRANSLUCENT
	#define S_TRANSLUCENT 1
	#endif
	
	#include "common/shared.hlsl"
	#include "procedural.hlsl"

	#define S_UV2 1
}

struct VertexInput
{
	#include "common/vertexinput.hlsl"
	float4 vColor : COLOR0 < Semantic( Color ); >;
};

struct PixelInput
{
	#include "common/pixelinput.hlsl"
	float3 vPositionOs : TEXCOORD14;
	float3 vNormalOs : TEXCOORD15;
	float4 vTangentUOs_flTangentVSign : TANGENT	< Semantic( TangentU_SignV ); >;
	float4 vColor : COLOR0;
	float4 vTintColor : COLOR1;
	#if ( PROGRAM == VFX_PROGRAM_PS )
		bool vFrontFacing : SV_IsFrontFace;
	#endif
};

VS
{
	#include "common/vertex.hlsl"

	PixelInput MainVs( VertexInput v )
	{
		
		PixelInput i;
		i.vPositionPs = float4(v.vPositionOs.xy, 0.0f, 1.0f );
		i.vPositionWs = float3(v.vTexCoord, 0.0f);
		
		return i;
		
	}
}

PS
{
	#include "common/pixel.hlsl"
	#include "postprocess/functions.hlsl"
	#include "postprocess/common.hlsl"
	RenderState( CullMode, F_RENDER_BACKFACES ? NONE : DEFAULT );
		
	Texture2D g_tColorBuffer < Attribute( "ColorBuffer" ); SrgbRead ( true ); >;
	float4 g_vColor < Attribute( "Color" ); Default4( 0.00, 0.00, 0.00, 1.00 ); >;
	float g_flTransition < Attribute( "Transition" ); Default1( 0 ); >;
	
	float4 MainPs( PixelInput i ) : SV_Target0
	{

		
		float4 l_0 = g_vColor;
		float2 l_1 = CalculateViewportUv( i.vPositionSs.xy );
		float l_2 = l_1.x;
		float l_3 = l_2 * 5;
		float l_4 = l_1.y;
		float l_5 = l_4 * 5;
		float2 l_6 = float2( l_3, l_5);
		float l_7 = g_flTime * 1;
		float l_8 = sin( l_7 );
		float l_9 = g_flTime * 0.5;
		float l_10 = sin( l_9 );
		float2 l_11 = float2( l_8, l_10);
		float l_12 = VoronoiNoise( l_1, 3.1415925, 10 );
		float2 l_13 = l_11 * float2( l_12, l_12 );
		float2 l_14 = l_6 + l_13;
		float l_15 = ValueNoise( l_14 );
		float l_16 = g_flTransition;
		float l_17 = saturate( ( l_16 - 0 ) / ( 1 - 0 ) ) * ( 1 - -1 ) + -1;
		float l_18 = ( l_17 - -1 ) / ( 0 - -1 ) * ( 1 - 0 ) + 0;
		float l_19 = saturate( ( l_15 - 0 ) / ( 1 - 0 ) ) * ( l_18 - l_17 ) + l_17;
		

		return float4( l_0.xyz, l_19 );
	}
}
