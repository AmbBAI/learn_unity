Shader "Custom/cg_multipass" {
	Properties {
	}
	SubShader {
		LOD 200
		
		Pass {
			Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
			ZTest LEqual

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct v2f
				{
					float4 pos : SV_POSITION;
				};

				v2f vert(float4 position : POSITION, float3 normal : NORMAL)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, position);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					return fixed4(1.,0.,0.,1.);
				}
			ENDCG
		}

		Pass {
			//Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
			Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
			ZTest Always
			ZWrite off

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct v2f
				{
					float4 pos : SV_POSITION;
				};

				v2f vert(float4 position : POSITION, float3 normal : NORMAL)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, position);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					return fixed4(0.,1.,0.,1.);
				}
			ENDCG
		}

		
		Pass {
			Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
			ZTest LEqual
			ZWrite off

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct v2f
				{
					float4 pos : SV_POSITION;
				};

				v2f vert(float4 position : POSITION, float3 normal : NORMAL)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, position);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					return fixed4(0.,0.,1.,1.);
				}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
