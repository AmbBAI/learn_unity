Shader "Custom/shadertoy_base" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {

		GLSLPROGRAM
	
		#include "UnityCG.glslinc"

		vec4 iResolution = _ScreenParams;
		float iGlobalTime = _Time.y;

#ifdef VERTEX
        void main()
        {
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
        }
#endif

#ifdef FRAGMENT
		void mainImage( out vec4 fragColor, in vec2 fragCoord )
		{
			vec2 uv = fragCoord.xy / iResolution.xy;
			fragColor = vec4(uv,0.5+0.5*sin(iGlobalTime),1.0);
		}

		void main()
		{
			mainImage(gl_FragColor, gl_FragCoord.xy);
		}
#endif
		ENDGLSL
		}
	}
}
