Shader "Custom/cg_shadow_transparent"
{
	Properties
	{
		_Color("Color", Color) = (1.,1.,1.,1.)
		_MainTex("Texture", 2D) = "white" {}
	}
	
	SubShader
	{
		Pass
		{
			Tags{
				"RenderType" = "Transparent"
				"IgnoreProjector" = "False"
				"Queue" = "Transparent"
				"LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
	
			struct appdata
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};
		
			struct v2f
			{
				float2 uv : TEXCOORD0;
				LIGHTING_COORDS(1,2)
				float4 pos : SV_POSITION;
			};
		
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
		
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.pos);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
		
			fixed4 frag(v2f i) : SV_Target
			{
				float attenuation = LIGHT_ATTENUATION(i);
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				fixed alpha = col.a;
				col = lerp(0, col, attenuation);
				col.a = alpha;
				return col;
			}
			ENDCG
		}
	}

	Fallback "Legacy Shaders/VertexLit"
}
