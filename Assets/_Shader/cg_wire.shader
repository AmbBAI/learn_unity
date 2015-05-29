Shader "Custom/cg_wire" {
	Properties {}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				struct v2f
				{
					float4 pos : SV_POSITION;
					fixed4 col : COLOR0;
				};

				v2f vert(float4 pos : POSITION, fixed4 col : COLOR0)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, pos);
					o.col = col;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					return i.col;
				}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
