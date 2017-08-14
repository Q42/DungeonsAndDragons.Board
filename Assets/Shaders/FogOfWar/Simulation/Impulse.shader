Shader "FogOfWar/Simulation/Impulse" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float _Aspect;
			float3 _Amount;
			float2 _ImpulsePos;
			float _Radius;
			
			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				float2 diff = i.uv - _ImpulsePos;
				diff.x *= _Aspect;
				float d = length(diff);
				
				float4 result  = float4(0, 0, 0, 0);
				if (d < _Radius) {
					float a = (_Radius - d);
					a = saturate(a);
					result = float4(_Amount, a);
				}
				
				return result;
			}
			ENDCG
		}
	}
}
