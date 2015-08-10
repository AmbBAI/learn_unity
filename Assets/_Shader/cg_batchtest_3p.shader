Shader "Custom/cg_batchtest_3p" {
	Properties {
		_MainTex ("mainTex", 2D) = "white"
	}
	SubShader {
		LOD 200
		
		Pass {
			Tags { "RenderType"="Opaque" "Queue" = "Geometry" }

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MainTex;

				struct vin
				{
					float4 position : POSITION;
					float3 normal : NORMAL;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					float3 normal : TEXCOORD1;
				};

				v2f vert(vin i)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, i.position);
					o.texcoord = i.texcoord;
					o.normal = mul ((float3x3)_Object2World, i.normal);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 c = tex2D(_MainTex, i.texcoord);
					float l = max(0., dot(i.normal, normalize(float3(1.,1.,-1.))));
					return c * l;
				}
			ENDCG
		}
	}
}