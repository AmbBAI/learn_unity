Shader "Custom/cg_outline" {
	Properties {
		_Outline ("Outline", Float) = 1.
		_OutlineColor ("Outline Color", Color) = (0.,0.,0.,1.)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
			ZTest Always
			ZWrite off

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				float _Outline;
				fixed4 _OutlineColor;

				struct v2f
				{
					float4 pos : SV_POSITION;
				};

				v2f vert(float4 position : POSITION, float3 normal : NORMAL)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, position);
					float3 n = mul ((float3x3)UNITY_MATRIX_IT_MV, normal);
					float2 offset = TransformViewToProjection(n.xy);
					o.pos.xy += offset * o.pos.z * _Outline;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					return _OutlineColor;
				}
			ENDCG
		}

		Pass {
			Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
			ZTest LEqual
			ZWrite on

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct v2f
				{
					float4 position : SV_POSITION;
					float3 normal : TEXCOORD1;
					fixed4 color : COLOR0;
					float3 lightDir : TEXCOORD0;
				};

				v2f vert(float4 position : POSITION, float3 normal : NORMAL, fixed4 color : COLOR)
				{
					v2f o;
					o.position = mul (UNITY_MATRIX_MVP, position);
					o.normal = normal;
					o.color = color;
					o.lightDir = WorldSpaceLightDir(position);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float lambert = saturate(dot(i.normal, normalize(i.lightDir)));
					lambert = lambert * 0.8 + 0.2;
					return i.color * lambert;
				}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
