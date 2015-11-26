Shader "Custom/cg_depth" {
	Properties{
	}
	SubShader{
		LOD 200

		Pass{
		Tags{ "RenderType" = "Opaque" }

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
		float4 projection : TEXCOORD0;
		float4 projMatrix : TEXCOORD1;
	};

	v2f vert(vin i)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, i.position);
		o.projection = o.pos;
		o.projMatrix = UNITY_MATRIX_P[2];
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
//		i.projection.z = i.projection.z / i.projection.w;
//		i.projection.z = Linear01Depth(i.projection.z);
//		return i.projection.zzzz;
		//i.projMatrix.w = i.projMatrix.w * _ZBufferParams.z / 2.;
		i.projMatrix.w = i.projMatrix.w * (1. / -.30018) / 2.;
		return i.projMatrix.wwww;
	}
		ENDCG
	}
	}
}