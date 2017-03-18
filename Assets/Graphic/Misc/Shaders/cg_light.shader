// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/cg_light" {
	Properties {
	}
	SubShader {
		LOD 200
		
		Pass {
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma glsl
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
					fixed4 vlit0 : TEXCOORD2;
					fixed4 vlit1 : TEXCOORD3;
					fixed4 vlit2 : TEXCOORD4;
					fixed4 vlit3 : TEXCOORD5;
					fixed3 shl : TEXCOORD6;
				};

				v2f vert(vin i)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, i.position);
					o.normal = mul ((float3x3)unity_ObjectToWorld, i.normal);

					fixed4 ndotl;
					ndotl.x = max(0., dot(i.normal, normalize(float3(unity_4LightPosX0.x, unity_4LightPosY0.x, unity_4LightPosZ0.x))));
					ndotl.y = max(0., dot(i.normal, normalize(float3(unity_4LightPosX0.y, unity_4LightPosY0.y, unity_4LightPosZ0.y))));
					ndotl.z = max(0., dot(i.normal, normalize(float3(unity_4LightPosX0.z, unity_4LightPosY0.z, unity_4LightPosZ0.z))));
					ndotl.w = max(0., dot(i.normal, normalize(float3(unity_4LightPosX0.w, unity_4LightPosY0.w, unity_4LightPosZ0.w))));
					o.vlit0 = unity_LightColor[0] *  (ndotl.x * unity_4LightAtten0.x);
					o.vlit1 = unity_LightColor[1] *  (ndotl.y * unity_4LightAtten0.y);
					o.vlit2 = unity_LightColor[2] *  (ndotl.z * unity_4LightAtten0.z);
					o.vlit3 = unity_LightColor[3] *  (ndotl.w * unity_4LightAtten0.w);
					o.shl = ShadeSH9(fixed4(normalize(o.normal), 1.));

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					//return fixed4(0.,0.,0.,0.);

					//float ndotl = max(0., dot(i.normal, normalize(_WorldSpaceLightPos0.xyz)));
					//return _LightColor0 * ndotl;

					//return i.vlit0;
					//return i.vlit1;
					//return i.vlit2;
					//return i.vlit3;

					return fixed4(i.shl, 1.);
				}
			ENDCG
		}

		Pass {
			Tags { "LightMode" = "ForwardAdd" }
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
					o.normal = mul ((float3x3)unity_ObjectToWorld, i.normal);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					//return fixed4(0.,0.,0.,0.);

					float l = max(0., dot(i.normal, normalize(_WorldSpaceLightPos0.xyz)));
					return _LightColor0 * l;
				}
			ENDCG
		}
	}
}