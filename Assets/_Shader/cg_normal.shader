Shader "Custom/cg_normal" {
	Properties {
	}
	SubShader {
		LOD 200
		
		Pass {
			Tags { "RenderType"="Opaque" }

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
					float3 viewNormal : TEXCOORD1;
					float3 worldNormal : TEXCOORD2;
				};

				v2f vert(vin i)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, i.position);
					o.viewNormal = mul (UNITY_MATRIX_MV, i.normal).xyz;
					o.worldNormal = mul (_Object2World, i.normal).xyz;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					//return fixed4(normalize(i.worldNormal) + fixed3(1.,1.,1.), 1.) / 2.;
					return fixed4(normalize(i.viewNormal) + fixed3(1.,1.,1.), 1.) / 2.;
					//float l = max(0., dot(i.normal, normalize(float3(1.,1.,-1.))));
					//return fixed4(l,l,l,1.);
				}
			ENDCG
		}
	}
}