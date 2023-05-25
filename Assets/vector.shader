Shader "dofdev/vector" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
 
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		
		Blend One One
		Cull Off
		ZWrite Off
		ZTest Less

		Pass {

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
 
				#include "UnityCG.cginc"
 
				struct appdata {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};
 
				struct v2f {
					float4 vertex : SV_POSITION;
					float3 normal : NORMAL;
				};

				float4 _Color;
 
				v2f vert (appdata v) {
					v2f o;
					// transform the vertex position into screen space
					o.vertex = UnityObjectToClipPos(v.vertex);
					// worldspace normal
					o.normal = UnityObjectToWorldNormal(v.normal);
					return o;
				}
 
				fixed4 frag (v2f i) : SV_Target {	
					// return _Color;
					// return float4(0,0,0,0);
					return float4(0, 0, 1, 0);
				}
			ENDCG
		}
	}
}