Shader "dofdev/color-cube"
{
	Properties
	{
		// _Outline ("Outline", Range(0, 1)) = 0.5
		
		[Toggle] _Linear ("Linear", Float) = 0
		[Toggle] _Lum ("Lum", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 pos : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 pos : TEXCOORD1;
			};

			float _Linear;
			float _Lum;

			float f(float x)
			{
				if (x >= 0.0031308)
					return (1 + 0.055) * pow(x, 1.0/2.4) - 0.055;
				
				return 12.92 * x;
			}

			float f_inv(float x)
			{
				if (x >= 0.04045)
					return pow((x + 0.055)/(1 + 0.055), 2.4);
				
				return x / 12.92;
			}

			// rgb to perception based blending
			float3 rgb2xyz(float3 rgb)
			{
				float3 xyz;
				xyz.x = 0.4124564 * rgb.r + 0.3575761 * rgb.g + 0.1804375 * rgb.b;
				xyz.y = 0.2126729 * rgb.r + 0.7151522 * rgb.g + 0.0721750 * rgb.b;
				xyz.z = 0.0193339 * rgb.r + 0.1191920 * rgb.g + 0.9503041 * rgb.b;
				return xyz;
			}

			float3 xyz2perception(float3 xyz) {
				float3 perception;
				perception.x = 0.8951 * xyz.x + 0.2664 * xyz.y - 0.1614 * xyz.z;
				perception.y = -0.7502 * xyz.x + 1.7135 * xyz.y + 0.0367 * xyz.z;
				perception.z = 0.0389 * xyz.x - 0.0685 * xyz.y + 1.0296 * xyz.z;
				return perception;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.pos = v.vertex.xyz;
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				// float3 obj_pos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
				// float3 pos = i.pos - obj_pos + float3(0.5, 0.5, 0.5);
				float3 pos = i.pos + float3(0.5, 0.5, 0.5);

				// pos to color
				float3 col = float3(pos);
				float r = col.r;
				float g = col.g;
				float b = col.b;
				
				// linear to sRGB
				if (_Linear == 1)
				{
					col = float3(f_inv(r), f_inv(g), f_inv(b));
				}

				// perceived luminance 
				// if (_Lum == 1)
				// {
				// 	float value = (r + r + g + g + g + b) / 6;
				// 	col = float3(
				// 		value * r / (r + g + b),
				// 		value * g / (r + g + b),
				// 		value * b / (r + g + b)
				// 	);
				// }

				// col = float4(0.5, 0.5, 0.5, 1);
				// if (col.g > 1.0) {
				// 	col = float3(0, 0, 0);
				// }

				

				

				return float4(col, 1);


				// return float4(r, g, b, 1);
			}
			ENDCG
		}
	}
}
