// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/cg_batchtest_2p" {
	Properties {
	}
	SubShader {
		LOD 200
		
		Pass {
			Tags { "RenderType"="Opaque" "Queue" = "Geometry" }

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct vin
				{
					float4 position : POSITION;
					float3 normal : NORMAL;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float3 normal : TEXCOORD1;
				};

				v2f vert(vin i)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, i.position);
					o.normal = mul ((float3x3)unity_ObjectToWorld, i.normal);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float l = max(0., dot(i.normal, normalize(float3(1.,1.,-1.))));
					return fixed4(l,l,l,1.);
				}
			ENDCG
		}
	}
}