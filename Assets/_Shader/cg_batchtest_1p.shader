Shader "Custom/cg_batchtest_1p" {
	Properties {
	}
	SubShader {
		LOD 200
		
		Pass {
			Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct vin
				{
					float4 position : POSITION;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
				};

				v2f vert(vin i)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, i.position);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					return fixed4(1.,1.,1.,1.);
				}
			ENDCG
		}
	}
}