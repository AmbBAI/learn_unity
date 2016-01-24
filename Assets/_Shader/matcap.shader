Shader "Custom/matcap" {
	Properties{
		_Color("BaseColor", Color) = (1,1,1,1)
		_BumpMap("Bumpmap (RGB)", 2D) = "bump" {}
		_MatCap("MatCap (RGB)", 2D) = "gray" {}
	}

		Subshader{
		Tags{ "RenderType" = "Opaque" }

		Pass{
			Name "BASE"
			Tags{ "LightMode" = "Always" }

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2	uv : TEXCOORD0;
				float3	TtoV0 : TEXCOORD1;
				float3	TtoV1 : TEXCOORD2;
				float3	TtoV2 : TEXCOORD3;
			};

			uniform float4 _BumpMap_ST;

			v2f vert(appdata_tan v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _BumpMap);

				TANGENT_SPACE_ROTATION;
				o.TtoV0 = mul(rotation, UNITY_MATRIX_IT_MV[0].xyz);
				o.TtoV1 = mul(rotation, UNITY_MATRIX_IT_MV[1].xyz);
				o.TtoV2 = mul(rotation, UNITY_MATRIX_IT_MV[2].xyz);
				return o;
			}

			uniform fixed4 _Color;
			uniform sampler2D _BumpMap;
			uniform sampler2D _MatCap;

			float4 frag(v2f i) : COLOR
			{
				float3 normal = UnpackNormal(tex2D(_BumpMap, i.uv));

				half3 vn;
				vn.x = dot(i.TtoV0, normal);
				vn.y = dot(i.TtoV1, normal);
				vn.z = dot(i.TtoV2, normal);
				vn = vn * 0.5 + 0.5;

				float4 matcapLookup = tex2D(_MatCap, vn.xy);
				matcapLookup.a = 1;
				return matcapLookup;
			}
				ENDCG
		}
	}
}
