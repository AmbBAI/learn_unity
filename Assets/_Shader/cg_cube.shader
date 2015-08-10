Shader "Custom/cg_cube" {
	Properties {
	  _CubeMap("Cube Map", Cube) = "" {}
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

		samplerCUBE _CubeMap;

		v2f vert(vin i)
		{
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, float4(i.position, 1.));
			o.normal = normalize(mul((float3x3)_Object2World, normalize(i.normal)));
			o.texcoord = i.texcoord;
			o.worldPos = mul(_Object2World, float4(i.position, 1.)).xyz;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			fixed3 viewDir = normalize (_WorldSpaceCameraPos.xyz - i.worldPos);

			// cube map
			fixed3 reflectDir = reflect (-viewDir, i.normal);
			fixed4 reflectCol = texCUBE (_CubeMap, reflectDir);

			return reflectCol;
		}

		ENDCG
		}
	}
}
