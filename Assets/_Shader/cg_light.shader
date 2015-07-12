Shader "Custom/cg_light" {
	Properties {
	}
	SubShader {
		LOD 200
		
		Pass {
			Tags {
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Lighting.cginc"

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
					o.normal = mul ((float3x3)_Object2World, i.normal);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float l = max(0., dot(i.normal, normalize(_WorldSpaceLightPos0.xyz)));
					return _LightColor0 * l;
				}
			ENDCG
		}

		Pass {
			Tags {
				"LightMode" = "ForwardAdd"
			}
			Blend One One

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Lighting.cginc"

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
					o.normal = mul ((float3x3)_Object2World, i.normal);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float l = max(0., dot(i.normal, normalize(_WorldSpaceLightPos0.xyz)));
					return _LightColor0 * l;
				}
			ENDCG
		}
	}
}