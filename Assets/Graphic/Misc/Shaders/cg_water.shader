// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/cg_water" {
    Properties {
    	WAVE_MOVEMENT ("WAVE_MOVEMENT", Float) = .1
    	WAVE_HEIGHT ("WAVE_HEIGHT", Float) = .1
        WATER_MOVEMENT ("WATER_MOVEMENT", Float) = 0.05
    	normal0 ("normal0", 2D) = "white" {}
    	u_1DivLevelWidth ("u_1DivLevelWidth", Float) = .5
    	u_1DivLevelHeight ("u_1DivLevelHeight", Float) = .5
    	u_lightPos ("u_lightPos", Vector) = (0.,10.,10.,0.)
    	SHORE_DARK ("SHORE_DARK", Color) = (0.,0.,1.,1.)
    	SHORE_LIGHT ("SHORE_LIGHT", Color) = (0.,1.,1.,1.)
    	SEA_DARK ("SEA_DARK", Color) = (0.,0.,1.,1.)
    	SEA_LIGHT ("SEA_LIGHT", Color) = (0.,1.,1.,1.)
        u_reflectionFactor ("u_reflectionFactor", Float) = 1.
        foam ("foam", 2D) = "white" {}
        _lightmap ("lightmap", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Transparent" }
        LOD 200
        
        Pass{
        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag
        #pragma target 3.0
		#pragma glsl

        float WAVE_MOVEMENT;
        float WAVE_HEIGHT;
        float WATER_MOVEMENT;
        fixed3 SEA_LIGHT;
        fixed3 SEA_DARK;
        fixed3 SHORE_LIGHT;
        fixed3 SHORE_DARK;
        float u_1DivLevelWidth;
        float u_1DivLevelHeight;
        sampler2D normal0;
        float3 u_lightPos;
		float u_reflectionFactor;
        sampler2D foam;
        sampler2D _lightmap;


        struct vertexInput
        {
        	float4 a_pos : POSITION;
			fixed4 a_color : COLOR0;
			float2 a_uv0 : TEXCOORD0;
        };

        struct fragmentInput{
            float4 position : SV_POSITION;
            float3 v_bumpUv1 : TEXCOORD0;
            float2 v_foamUv : TEXCOORD1;
            float2 v_worldPos : TEXCOORD2;
			fixed3 v_darkColor : TEXCOORD3;
			fixed3 v_lightColor : TEXCOORD4;
            float4 v_wave : TEXCOORD5;
        };

        fragmentInput vert(vertexInput i) {

			fragmentInput o;
			float4 pos = i.a_pos;
			float u_time = _Time.y;
        	float animTime = i.a_uv0.y + u_time;
        	float wave = cos(animTime);
        	float waveHeightFactor = (wave + 1.) * .5;
        	pos.x += waveHeightFactor * WAVE_MOVEMENT * i.a_color.g * i.a_color.b;
        	pos.y += wave * WAVE_HEIGHT * i.a_color.b;
            float4 w_pos =  mul (unity_ObjectToWorld, pos);
        	o.position = mul (UNITY_MATRIX_MVP, pos);

			float maxValue = 0.55;//0.5;
			o.v_wave.x = 1.0 - (i.a_color.a - maxValue) * (1.0 / maxValue);
			o.v_wave.x = o.v_wave.x * o.v_wave.x;
			o.v_wave.x = o.v_wave.x * 0.8 + 0.2;
			o.v_wave.x -= wave * i.a_color.b * 0.1;
			o.v_wave.x = min(1.0, o.v_wave.x);

        	float2 texcoordMap = float2(i.a_pos.x * u_1DivLevelWidth, i.a_pos.z * u_1DivLevelHeight) * 40.0;
			o.v_bumpUv1.xy = texcoordMap * 0.5 + float2(u_time * WATER_MOVEMENT, 0.) * 1.5;
            o.v_foamUv = (texcoordMap + float2(0., u_time * WATER_MOVEMENT)) * 0.55;

			float3 lightDir = normalize(float3(-1.0, 1.0, 0.0));
			float3 lightVec = normalize(u_lightPos - w_pos.xyz);
			o.v_wave.z = (1.0 - abs(dot(lightDir, lightVec)));
			o.v_wave.z = o.v_wave.z * 0.2 + (o.v_wave.z * o.v_wave.z) * 0.8;
			o.v_wave.z = clamp(o.v_wave.z + 1.1 - (length(u_lightPos - w_pos.xyz) * 0.008), 0.0, 1.0);
			o.v_wave.w = (1.0 + (1.0 - o.v_wave.z * 0.5) * 7.0);

            o.v_worldPos = float2(w_pos.x * u_1DivLevelWidth, w_pos.z * u_1DivLevelHeight) + 0.5;

		    o.v_wave.y = (cos((i.a_pos.x + u_time) * i.a_pos.z * 0.003 + u_time) + 1.0) * 0.5;

			float blendFactor = 1.0 - min(i.a_color.a * 1.6, 1.);
			
			float tx = w_pos.x * u_1DivLevelWidth - 0.5;
			float ty = w_pos.z * u_1DivLevelHeight - 0.5;
			
			float tmp = (tx * tx + ty * ty) / (0.75 * 0.75);
			float blendFactorMul = step(1.0, tmp);
			tmp = pow(tmp, 3.0);
			// Can't be above 1.0, so no clamp needed
			float blendFactor2 = max(blendFactor - (1.0 - tmp) * 0.5, 0.);
			blendFactor = lerp(blendFactor2, blendFactor, blendFactorMul);
		
			o.v_darkColor = lerp(SHORE_DARK, SEA_DARK, blendFactor);
			o.v_lightColor = lerp(SHORE_LIGHT, SEA_LIGHT, blendFactor);

            o.v_bumpUv1.z = ((1.0 - i.a_color.a) + blendFactor) * 0.5;//blendFactor;
            //// Put to log2 here because there's pow(x,y)*z in the fragment shader calculated as exp2(log2(x) * y + log2(z)), where this is is the log2(z)
            o.v_bumpUv1.z = log2(o.v_bumpUv1.z);

            return o;
        }

        fixed4 frag(fragmentInput i) : COLOR {
            fixed4 normalMapValue = tex2D(normal0, i.v_bumpUv1.xy);
            fixed4 fragColor = fixed4(lerp(i.v_lightColor, i.v_darkColor, (normalMapValue.x * i.v_wave.y) + (normalMapValue.y * (1.0 - i.v_wave.y))), i.v_wave.x);
            fragColor += exp2(log2(((normalMapValue.z * i.v_wave.y) + (normalMapValue.w * (1.0 - i.v_wave.y))) * i.v_wave.z) * i.v_wave.w + i.v_bumpUv1.z) * u_reflectionFactor;

            float3 lightmapValue = tex2D(_lightmap, i.v_worldPos).rgb * float3(tex2D(foam, i.v_foamUv).r * 1.5, 1.3, 1.0);
            fragColor = lerp(fragColor, float4(0.92, 0.92, 0.92, lightmapValue.x), min(0.92, lightmapValue.x)) * lightmapValue.yyyz;
            return fragColor;
        }

        ENDCG
        }
    }
}
