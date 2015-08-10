Shader "Custom/cg_font_df" {
	Properties {
		_MainTex ("Main Tex", 2D) = "white"{}
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				sampler2D _MainTex;

				struct v2f
				{
					float4 pos : SV_POSITION;
					fixed4 col : COLOR0;
					float2 texc : TEXCOORD0;
				};

				v2f vert(float4 pos : POSITION, fixed4 col : COLOR0, float2 texc : TEXCOORD0)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, pos);
					o.col = col;
					o.texc = texc;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = i.col;
					float smoothing = 1./16.;
					float distance = tex2D(_MainTex, i.texc).a;
					col.a = smoothstep(.5 - smoothing, .5 + smoothing, distance);
					return col;
				}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
