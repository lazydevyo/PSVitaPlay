// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/AVProLiveCamera/CompositeYUY2_2_RGBA"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TextureWidth ("Texure Width", Float) = 256.0
	}
	SubShader 
	{
		Pass
		{ 
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
		
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers flash xbox360 ps3 gles
//#pragma fragmentoption ARB_precision_hint_fastest
#pragma fragmentoption ARB_precision_hint_nicest
#pragma multi_compile SWAP_RED_BLUE_ON SWAP_RED_BLUE_OFF
#pragma multi_compile HORIZONTAL_FLIP_ON HORIZONTAL_FLIP_OFF
#pragma multi_compile AVPRO_GAMMACORRECTION AVPRO_GAMMACORRECTION_OFF
#include "UnityCG.cginc"
#include "AVProLiveCamera_Shared.cginc"

uniform sampler2D _MainTex;
float _TextureWidth;
#if UNITY_VERSION >= 530
uniform float4 _MainTex_ST2;
#else
uniform float4 _MainTex_ST;
#endif
float4 _MainTex_TexelSize;

struct v2f {
	float4 pos : POSITION;
	float3 uv : TEXCOORD0;
};

v2f vert( appdata_img v )
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	
#if UNITY_VERSION >= 530
	o.uv.xy = (v.texcoord.xy * _MainTex_ST2.xy + _MainTex_ST2.zw);
#else
	o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
#endif
	
	// On D3D when AA is used, the main texture & scene depth texture
	// will come out in different vertical orientations.
	// So flip sampling of the texture when that is the case (main texture
	// texel size will have negative Y).
	#if SHADER_API_D3D9
	if (_MainTex_TexelSize.y < 0)
	{
		o.uv.y = 1-o.uv.y;
	}
	#endif
	
	o.uv.z = v.vertex.x * _TextureWidth * 0.5;

	return o;
}

float4 frag (v2f i) : COLOR
{
	float4 col = tex2D(_MainTex, i.uv.xy);
#if defined(SWAP_RED_BLUE_ON)
	col = col.bgra;
#endif

#if defined(HORIZONTAL_FLIP_ON)
	col = col.bgra;
#endif

	//yuyv
	float y = col.z;
	float u = col.y;
	float v = col.w;
	
	if (frac(i.uv.z) > 0.5 )
	{
		// ODD PIXELS
		y = col.x;
		
		/*float4 col2 = tex2D(_MainTex, i.uv.xy + float2(_MainTex_TexelSize.x, 0.0));
#if defined(SWAP_RED_BLUE_ON)
		col2 = col2.bgra;
#endif
		u = (col.y + col2.y) * 0.5;
		v = (col.w + col2.w) * 0.5;*/
	}

	float4 oCol = convertYUV(y, u, v);	

#if defined(AVPRO_GAMMACORRECTION)
	oCol.rgb = pow(oCol.rgb, 1.2);
#endif

	return float4(oCol.rgb, 1);
} 

ENDCG
		}
	}
	
	FallBack Off
}
