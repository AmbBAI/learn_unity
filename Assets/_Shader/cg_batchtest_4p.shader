Shader "Custom/cg_batchtest_4p" {
	Properties {
		_MainTex ("mainTex", 2D) = "white"
		_BumpTex ("bumpTex", 2D) = "bump"
	}
	SubShader {
		LOD 200
		
		Pass {
			Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				sampler2D _BumpTex;

				struct vin
				{
					float4 position : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					float3 tspace0 : TEXCOORD1;
					float3 tspace1 : TEXCOORD2;
					float3 tspace2 : TEXCOORD3;
				};

				v2f vert(vin i)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, i.position);
					o.texcoord = i.texcoord;
					fixed3 worldNormal = mul((float3x3)_Object2World, i.normal);
					fixed3 worldTangent = mul((float3x3)_Object2World, i.tangent.xyz);
					fixed3 worldBinormal = cross(worldNormal, worldTangent) * i.tangent.w;
					o.tspace0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
					o.tspace1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
					o.tspace2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 c = tex2D(_MainTex, i.texcoord);

					fixed3 normal = UnpackNormal(tex2D(_BumpTex, i.texcoord));
					fixed3 worldN;
					worldN.x = dot(i.tspace0, normal);
					worldN.y = dot(i.tspace1, normal);
					worldN.z = dot(i.tspace2, normal);
					float l = max(0., dot(worldN, normalize(float3(1.,1.,-1.))));
					return c * l;
				}
			ENDCG
		}
	}
}