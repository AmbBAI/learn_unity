Shader "Custom/cg_cover" {
	Properties {
		_RimColor ("Rim Color", Color) = (1.,1.,1.,1.)
	}
	SubShader {

		LOD 200
		
		Pass {
			Tags {"RenderType"="Transparent" "Queue"="Transparent"}
			ZWrite off
			ZTest Greater
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				fixed4 _RimColor;

				struct v2f
				{
					float4 pos : SV_POSITION;
					fixed4 col : COLOR0;
				};

				v2f vert(float4 position : POSITION, float3 normal : NORMAL)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, position);
					float3 viewDir = WorldSpaceViewDir(position);
					o.col = _RimColor;
					o.col.a = 1.0 - saturate(dot(normalize(viewDir), normal));
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					return i.col;
				}
			ENDCG
		}

		Pass {
			Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
			ZWrite on
			ZTest LEqual

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct v2f
				{
					float4 position : SV_POSITION;
					float3 normal : NORMAL;
					fixed4 color : COLOR0;
				};

				v2f vert(float4 position : POSITION, float3 normal : NORMAL, fixed4 color : COLOR)
				{
					v2f o;
					o.position = mul (UNITY_MATRIX_MVP, position);
					o.normal = normal;
					o.color = color;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float lambert = saturate(dot(i.normal, WorldSpaceLightDir(i.position)));
					lambert = lambert * 0.8 + 0.2;
					return i.color * lambert;
				}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
