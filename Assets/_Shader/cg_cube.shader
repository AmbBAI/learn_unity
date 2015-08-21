Shader "Custom/cg_cube" {
	Properties {
	  _MainTex("Main Tex", 2D) = "black"
	  _CubeMap("Cube Map", Cube) = "" {}
	  _Reflection ("Reflection Power", Range(0, 1)) = 0.
	  _FresnelPow("Fresnel Power", Range(0, 2)) = .25
	  _FresnelFalloff("Fresnel Falloff", Range(0, 10)) = 4.
	}
	SubShader {
		Tags { "QUEUE"="Geometry" "RenderType"="Opaque" }
		LOD 200

        
        Pass{
        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag

		#include "UnityCG.cginc"
		#include "Lighting.cginc"

        struct vin
        {
        	float3 position : POSITION;
        	float3 normal : NORMAL;
        	float2 texcoord : TEXCOORD0;
        };

        struct v2f
        {
        	float4 pos : SV_POSITION;
        	float3 normal : TEXCOORD4;
			float2 texcoord : TEXCOORD0;
			float3 worldPos : TEXCOORD1;
        };

		sampler2D _MainTex;
		samplerCUBE _CubeMap;
		float _Reflection;
		float _FresnelPow;
		float _FresnelFalloff;

		v2f vert(vin i)
		{
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, float4(i.position, 1.));
			o.normal = normalize(mul((float3x3)_Object2World, normalize(i.normal)));
			o.texcoord = i.texcoord * float2(5., 1.);
			o.worldPos = mul(_Object2World, float4(i.position, 1.)).xyz;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			float l = max(0., dot(i.normal, normalize(_WorldSpaceLightPos0.xyz)));
			fixed4 color = tex2D(_MainTex, i.texcoord) * l;

			fixed3 viewDir = normalize (_WorldSpaceCameraPos.xyz - i.worldPos);

			fixed3 reflectDir = reflect (-viewDir, i.normal);
			fixed4 reflectCol = texCUBE (_CubeMap, reflectDir);
			float fresnel = pow (1. - saturate (abs (dot (viewDir, i.normal))), _FresnelFalloff) * _FresnelPow;
			reflectCol *= saturate (fresnel + _Reflection);

			return color + reflectCol + fresnel * reflectCol;
		}

		ENDCG
		}
	}
}
