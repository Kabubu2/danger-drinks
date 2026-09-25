
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
	#define S_TRANSLUCENT 0
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
		
		PixelInput i = ProcessVertex( v );
		i.vPositionOs = v.vPositionOs.xyz;
		i.vColor = v.vColor;
		
		ExtraShaderData_t extraShaderData = GetExtraPerInstanceShaderData( v.nInstanceTransformID );
		i.vTintColor = extraShaderData.vTint;
		
		VS_DecodeObjectSpaceNormalAndTangent( v, i.vNormalOs, i.vTangentUOs_flTangentVSign );
		return FinalizeVertex( i );
		
	}
}

PS
{
	#include "common/pixel.hlsl"
	RenderState( CullMode, F_RENDER_BACKFACES ? NONE : DEFAULT );
	
	float4 MainPs( PixelInput i ) : SV_Target0
	{
		
		Material m = Material::Init( i );
		m.Albedo = float3( 1, 1, 1 );
		m.Normal = float3( 0, 0, 1 );
		m.Roughness = 1;
		m.Metalness = 0;
		m.AmbientOcclusion = 1;
		m.TintMask = 1;
		m.Opacity = 1;
		m.Emission = float3( 0, 0, 0 );
		m.Transmission = 0;
		
		float l_0 = sin( g_flTime );
		float l_1 = g_flTime * 0.12;
		float l_2 = sin( l_1 );
		float l_3 = g_flTime * 1.444;
		float l_4 = sin( l_3 );
		float3 l_5 = float3( l_0, l_2, l_4 );
		float l_6 = g_flTime * 0.0311;
		float l_7 = sin( l_6 );
		float l_8 = l_7 * 20;
		float l_9 = l_7 * 40;
		float2 l_10 = float2( l_8, l_9);
		float2 l_11 = i.vTextureCoords.xy * float2( 1, 1 );
		float2 l_12 = l_10 + l_11;
		float l_13 = VoronoiNoise( l_12, 4.995462, 6.4884024 );
		float l_14 = g_flTime * 0.021;
		float l_15 = sin( l_14 );
		float l_16 = l_15 * 16;
		float l_17 = l_15 * 12;
		float2 l_18 = float2( l_16, l_17);
		float2 l_19 = i.vTextureCoords.xy * float2( 1, 1 );
		float2 l_20 = l_18 + l_19;
		float l_21 = VoronoiNoise( l_20, 4.995462, 7.816764 );
		float l_22 = g_flTime * 0.0112;
		float l_23 = sin( l_22 );
		float l_24 = l_23 * 11;
		float l_25 = l_23 * 35;
		float2 l_26 = float2( l_24, l_25);
		float2 l_27 = i.vTextureCoords.xy * float2( 1, 1 );
		float2 l_28 = l_26 + l_27;
		float l_29 = VoronoiNoise( l_28, 6.28319, 2.8888178 );
		float l_30 = l_21 * l_29;
		float l_31 = l_13 * l_30;
		float l_32 = saturate( ( l_31 - 0 ) / ( 0.15018024 - 0 ) ) * ( 1 - 0.44182876 ) + 0.44182876;
		float3 l_33 = l_5 * float3( l_32, l_32, l_32 );
		
		m.Albedo = l_33;
		m.Opacity = 1;
		m.Roughness = 1;
		m.Metalness = 0;
		m.AmbientOcclusion = 1;
		
		
		m.AmbientOcclusion = saturate( m.AmbientOcclusion );
		m.Roughness = saturate( m.Roughness );
		m.Metalness = saturate( m.Metalness );
		m.Opacity = saturate( m.Opacity );
		
		// Result node takes normal as tangent space, convert it to world space now
		m.Normal = TransformNormal( m.Normal, i.vNormalWs, i.vTangentUWs, i.vTangentVWs );
		
		// for some toolvis shit
		m.WorldTangentU = i.vTangentUWs;
		m.WorldTangentV = i.vTangentVWs;
		m.TextureCoords = i.vTextureCoords.xy;
				
		return ShadingModelStandard::Shade( m );
	}
}
