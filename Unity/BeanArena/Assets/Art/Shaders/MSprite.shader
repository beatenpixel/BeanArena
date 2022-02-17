Shader "MicroCrew/MSprite"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile _ PIXELSNAP_ON
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
					float2 worldPos : TEXCOORD1;
				};

				fixed4 _Color;

				float3 mod289(float3 x) {
					return x - floor(x * (1.0 / 289.0)) * 289.0;
				}

				float2 mod289(float2 x) {
					return x - floor(x * (1.0 / 289.0)) * 289.0;
				}

				float3 permute(float3 x) {
					return mod289(((x * 34.0) + 10.0) * x);
				}

				float snoise(float2 v) {
					const float4 C = float4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
						0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
						-0.577350269189626,  // -1.0 + 2.0 * C.x
						0.024390243902439); // 1.0 / 41.0
  // First corner
					float2 i = floor(v + dot(v, C.yy));
					float2 x0 = v - i + dot(i, C.xx);

					// Other corners
					float2 i1;
					//i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
					//i1.y = 1.0 - i1.x;
					i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
					// x0 = x0 - 0.0 + 0.0 * C.xx ;
					// x1 = x0 - i1 + 1.0 * C.xx ;
					// x2 = x0 - 1.0 + 2.0 * C.xx ;
					float4 x12 = x0.xyxy + C.xxzz;
					x12.xy -= i1;

					// Permutations
					i = mod289(i); // Avoid truncation effects in permutation
					float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0))
						+ i.x + float3(0.0, i1.x, 1.0));

					float3 m = max(0.5 - float3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
					m = m * m;
					m = m * m;

					// Gradients: 41 points uniformly over a line, mapped onto a diamond.
					// The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

					float3 x = 2.0 * frac(p * C.www) - 1.0;
					float3 h = abs(x) - 0.5;
					float3 ox = floor(x + 0.5);
					float3 a0 = x - ox;

					// Normalise gradients implicitly by scaling m
					// Approximation of: m *= inversesqrt( a0*a0 + h*h );
					m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);

					// Compute final noise value at P
					float3 g;
					g.x = a0.x * x0.x + h.x * x0.y;
					g.yz = a0.yz * x12.xz + h.yz * x12.yw;
					return 130.0 * dot(m, g);
				}

				float myrandom(float2 uv) {
					return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
				} 

				v2f vert(appdata_t IN) {
					v2f OUT;				
					
					float2 startWorldPos = mul(unity_ObjectToWorld, IN.vertex);
					float2 seedX = float2(startWorldPos.x, startWorldPos.y) + _Time.y * 0.49237;
					float2 seedY = float2(startWorldPos.y, startWorldPos.x) + _Time.y * 0.4399978;

					seedX = frac(seedX);
					seedY = frac(seedY);

					IN.vertex += float4(snoise(seedX), snoise(seedY),0,0) * 0.015;

					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color * _Color;
					OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex);

					//OUT.vertex = UnityPixelSnap(OUT.vertex);

					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif

					return OUT;
				}

				sampler2D _MainTex;
				sampler2D _AlphaTex;
				float _AlphaSplitEnabled;				

				fixed4 SampleSpriteTexture(float2 uv) {
					fixed4 color = tex2D(_MainTex, uv);

	#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
					if (_AlphaSplitEnabled)
						color.a = tex2D(_AlphaTex, uv).r;
	#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

					return color;
				}

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
					c.rgb *= c.a;
					const float _YBase = -1.8;

					float v = IN.worldPos.y; 

					/*
					float noiseOffset = snoise(IN.worldPos.xy * 0.5) * 0.5 + 0.5;
					v += noiseOffset * 0.3;
					*/

					float fadeP = clamp((v- _YBase) * 1.3, 0, 1);
					c.rgb *= lerp(0.7,1,fadeP);
					return c;
				}
			ENDCG
			}
		}
}