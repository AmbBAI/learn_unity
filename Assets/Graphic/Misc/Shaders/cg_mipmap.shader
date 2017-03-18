Shader "Custom/cg_mipmap" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		sampler2D _MainTex;

		struct v2f
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		v2f vert(float4 pos : POSITION, float2 uv : TEXCOORD0)
		{
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, pos);
			o.uv = uv;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			//half4 c = tex2Dlod (_MainTex, float4(i.uv * 10., 0., 3.));
			//half4 c = tex2Dbias (_MainTex, float4(i.uv * 10., 0., 1.));
			half4 c = tex2D (_MainTex, i.uv * 10.);	
			fixed v = (c.r - 0.8) * 5.;
			return fixed4(v,v,v,1.);
		}

		ENDCG
		}
	} 
}
