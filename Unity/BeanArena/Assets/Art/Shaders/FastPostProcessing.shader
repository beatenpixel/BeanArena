Shader "Demonixis/FastPostProcessing"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ShadowTex("ShadowTexture",2D) = "white" {}
		_VignetteColor("VignetteColor", Color) = (0,0,0,0)
	}

		CGINCLUDE
#include "UnityCG.cginc"
#pragma fragmentoption ARB_precision_hint_nicest

	uniform sampler2D _MainTex;
	uniform sampler2D _ShadowTex;
	uniform sampler2D _MNoiseTex;
	uniform half4 _MainTex_TexelSize;
	uniform	half4 _MainTex_ST;
	uniform float _SharpenSize;
	uniform float _SharpenIntensity;
	uniform float _BloomSize;
	uniform float _BloomAmount;
	uniform float _BloomPower;
	uniform float _Exposure;
	sampler2D _UserLutTex;
	uniform half4 _UserLutParams;

	float4x4 _CameraToWorldMatrix;
	uniform float _MapBaseLineCameraSpaceY;
	uniform float _MapBaseLineWorldY;
	uniform float _GrayscaleValue;
	fixed4 _VignetteColor;

	struct v2f_data {
		float4 pos : SV_POSITION;
		float2  uv  : TEXCOORD0;
		float4 vertexWorldPos : COLOR0;
#if UNITY_UV_STARTS_AT_TOP
		float2  uv2 : TEXCOORD1;
#endif
	};

	v2f_data vertFunction(appdata_img v) {
		v2f_data o;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);

#if UNITY_UV_STARTS_AT_TOP
		o.uv2 = o.uv;
		if (_MainTex_TexelSize.y < 0.0)
			o.uv.y = 1.0 - o.uv.y;
#endif
		float4 d = mul(_CameraToWorldMatrix, float4((v.texcoord.x) * 2 - 1, (v.texcoord.y) * 2 - 1, 0.5, 1));
		o.vertexWorldPos = float4(d.xy,0,0);

		return o;
	}

	half4 fragFunction(v2f_data i) : SV_Target
	{
		float2 uv = i.uv;
#if UNITY_UV_STARTS_AT_TOP
		uv = i.uv2;
#endif

		half3 col = tex2D(_MainTex, uv).rgb;

/*		BLOOM
		float size = 1 / _BloomSize;
		float4 sum = 0;
		float3 bloom;

		for (int i = -3; i < 3; i++) {
			sum += tex2D(_MainTex, uv + float2(-1, i) * size) * _BloomAmount;
			sum += tex2D(_MainTex, uv + float2(0, i) * size) * _BloomAmount;
			sum += tex2D(_MainTex, uv + float2(1, i) * size) * _BloomAmount;
		}

		if (col.r < 0.3 && col.g < 0.3 && col.b < 0.3) {
			bloom = sum.rgb * sum.rgb * 0.012 + col;
		} else {
			if (col.r < 0.5 && col.g < 0.5 && col.b < 0.5) {
				bloom = sum.xyz * sum.xyz * 0.009 + col;
			} else {
				bloom = sum.xyz * sum.xyz * 0.0075 + col;
			}
		}

		col = lerp(col, bloom, _BloomPower);
*/

		// Dithering
		// Interleaved Gradient Noise from http://www.iryoku.com/next-generation-post-processing-in-call-of-duty-advanced-warfare (slide 122)
		// 27
		const float ditherSize = 1;
		float3 magic = float3(0.06711056, 0.00583715, 52.9829189);
		float gradient = frac(magic.z * frac(dot(uv / _MainTex_TexelSize * ditherSize, magic.xy))) / 255.0;
		col.rgb += gradient.xxx * 20;

		// Vignette
		half2 dst = (uv - 0.5) * 1.25;
		dst.x = 1 - dot(dst, dst)*0.5;
		//col.rgb *= dst.x;
		col.rgb = lerp(col.rgb, _VignetteColor.rgb, (1 - dst.x));

		// Shadows
		//float offsetX = sin(_Time.y) * 0.15;
		float2 reflectionUv = float2(uv.x, _MapBaseLineCameraSpaceY - (uv.y - _MapBaseLineCameraSpaceY)*2);

		float offsetX = 0.2;

		half4 noiseSample = tex2D(_MNoiseTex, reflectionUv * 0.3);
		float fadeP = (_MapBaseLineCameraSpaceY - uv.y) / _MapBaseLineCameraSpaceY;

		half4 shadowSample = tex2D(_ShadowTex, float2(uv.x - offsetX * fadeP, _MapBaseLineCameraSpaceY - (uv.y - _MapBaseLineCameraSpaceY)*3));		

		fixed below = step(_MapBaseLineWorldY, i.vertexWorldPos.y);
		float shadowSample2 = step(0.5,shadowSample.a);
		col.rgb *= 1 - (1 - below) * shadowSample2 * clamp(shadowSample * 100,0,1) * 0.3 * (1 - fadeP);

		// Reflections
		
		/* V1
		
		*/

		
		if(true) {			
			half4 reflectionSample = tex2D(_ShadowTex, reflectionUv);
			float reflectionFadeP = clamp((_MapBaseLineCameraSpaceY - uv.y) / _MapBaseLineCameraSpaceY * 3,0,1) * noiseSample.x * 0.5;

			//col.rgb = lerp(col.rgb, fixed3(reflectionFadeP,reflectionFadeP,reflectionFadeP), (1-below));
			col.rgb = lerp(col.rgb, reflectionSample, (1 - below) * reflectionFadeP);			
			//col.rgb = lerp(col.rgb, noiseSample, (1-below));
		} else {
			float2 reflectionUv = float2(uv.x, _MapBaseLineCameraSpaceY - (uv.y - _MapBaseLineCameraSpaceY)*2);
			half4 reflectionSample = tex2D(_MainTex, reflectionUv*0.1);
			col.rgb = lerp(col.rgb, reflectionSample * 0.5, (1 - below) * 0.3);

			//col.rgb *= noise(reflectionUv);
		}		
		

		/*
		if (i.vertexWorldPos.y < _MapBaseLineWorldY) {		

				if (shadowSample.a > 0.5) {
					col.rgb *= 1 - clamp(shadowSample * 100,0,1) * 0.3 * (1 - fadeP);
					//col.r = 1 - fadeP;
					//col.g = fadeP;
				}
			}
		*/

		// Grayscale
		col.rgb = lerp(col.rgb, (col.r + col.g + col.b) * 0.3333, _GrayscaleValue);

		//col.rgb = fadeP;

		//col.rgb = 1 - col.rgb;

		return half4(col, 1.0);
	}

		ENDCG
		SubShader {
		Cull Off ZWrite Off ZTest Always

			Pass
		{
CGPROGRAM
#pragma vertex vertFunction
#pragma fragment fragFunction
ENDCG
		}
	}
}