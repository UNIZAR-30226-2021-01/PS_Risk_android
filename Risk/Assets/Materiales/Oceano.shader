Shader "Unlit/Oceano"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DepthCol ("Depth Color", Color) = (0,0,0,1)
		_SurfCol ("Surface Color", Color) = (0.5,0.5,0.5,1)
		_WaveCol ("Wave Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			float random(float2 uv){
				return frac(sin(dot(uv, float2(15.3548, 714.1235)))*4357.1534);
			}

			float noise(float2 uv){
				float2 i = floor(uv);
				float2 f = smoothstep(0, 1, frac(uv));
				
				float a = random(i);
				float b = random(i+float2(1, 0));
				float c = random(i+float2(0, 1));
				float d = random(i+float2(1, 1));
				
				return lerp(lerp(a, b,f.x), lerp(c, d,f.x), f.y);
			}
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _DepthCol, _SurfCol, _WaveCol;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target {
				float2 uv = i.uv*5;
				float n = 500;
				float2 gid = floor(uv*n)/n;
				float2 gid2 = floor(uv*200)/100;
				float2 guv = frac(uv*n);
				float wn = 75;
				float waves = step(frac(gid.y*wn+sin(sin(uv.y*20+_Time.y+uv.x*10)*1.25+uv.x*wn)/3), 0.15);
				float n1 = floor(10*noise(gid2.xyyx+float2(_Time.x/3, 0)))/10;
				//float4 background = lerp(float4(0.1, 0.2, 0.7, 1), float4(0.3, 0.4, 0.6, 1), floor(3*noise(floor(noise(gid*2-_Time.x/5)*300+uv*300)/100+_Time.x))/3);
				return lerp(_DepthCol, _SurfCol, n1)+_WaveCol*waves*0.1;
			}
			ENDCG
		}
	}
}
