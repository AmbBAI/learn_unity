#ifndef UNITY_GLOBAL_ILLUMINATION_INCLUDED
#define UNITY_GLOBAL_ILLUMINATION_INCLUDED

// Functions sampling light environment data (lightmaps, light probes, reflection probes), which is then returned as the UnityGI struct.

#define IMPROVED_BAKED_AND_REALTIME_SHADOW_MIXING 1

#include "UnityStandardBRDF.cginc"
#include "UnityStandardUtils.cginc"

inline half3 DecodeDirectionalSpecularLightmap (half3 color, fixed4 dirTex, half3 normalWorld, bool isRealtimeLightmap, fixed4 realtimeNormalTex, out UnityLight o_light)
{
	o_light.color = color;
	o_light.dir = dirTex.xyz * 2 - 1;

	// The length of the direction vector is the light's "directionality", i.e. 1 for all light coming from this direction,
	// lower values for more spread out, ambient light.
	half directionality = max(0.001, length(o_light.dir));
	o_light.dir /= directionality;

	#ifdef DYNAMICLIGHTMAP_ON
	if (isRealtimeLightmap)
	{
		// Realtime directional lightmaps' intensity needs to be divided by N.L
		// to get the incoming light intensity. Baked directional lightmaps are already
		// output like that (including the max() to prevent div by zero).
		half3 realtimeNormal = realtimeNormalTex.xyz * 2 - 1;
		o_light.color /= max(0.125, dot(realtimeNormal, o_light.dir));
	}
	#endif

	o_light.ndotl = LambertTerm(normalWorld, o_light.dir);

	// Split light into the directional and ambient parts, according to the directionality factor.
	half3 ambient = o_light.color * (1 - directionality);
	o_light.color = o_light.color * directionality;

	// Technically this is incorrect, but helps hide jagged light edge at the object silhouettes and
	// makes normalmaps show up.
	ambient *= o_light.ndotl;
	return ambient;
}

inline half3 MixLightmapWithRealtimeAttenuation (half3 lightmap, half attenuation, fixed4 bakedColorTex, half3 normalWorld)
{
	// Let's try to make realtime shadows work on a surface, which already contains
	// baked lighting and shadowing from the current light.

#if IMPROVED_BAKED_AND_REALTIME_SHADOW_MIXING
	half shadowStrength = _LightShadowData.x;
	half attenuationUnaffectedByShadowStrength = max(0, attenuation - shadowStrength);

	// Calculate possible value in the shadow by two very distinct ways:
	// 1) by subtracting estimated light contribution from the places occluded by realtime shadow:
	//		a) preserves other baked lights and light bounces
	//		b) eliminates shadows on the geometry facing away from the light
	//		BUT in case of (ShadowStrength < 1) subtracts light in the areas that are not covered by realtime shadow
	// 2) by attenuating lightmap - usually produces results that are:
	// 		a) too dark in region where baked and realtime shadow overlap
	//		b) destroys other baked lights
	//		c) shadows are visible on the geometry facing away from the light
	// 		BUT it handles (ShadowStrength < 1) better
	// Then use min/max arbiter to get a solution.


	// 1) Gives good estimate of illumination as if light would've been shadowed during the bake.
	//    Preserves bounce and other baked lights
	//    No shadows on the geometry facing away from the light
	half ndotl = LambertTerm (normalWorld, _WorldSpaceLightPos0.xyz);
	half3 estimatedLightContributionMaskedByInverseOfShadow = ndotl * (1-attenuationUnaffectedByShadowStrength) * _LightColor0.rgb;
	half3 subtractedLightmap = lightmap - estimatedLightContributionMaskedByInverseOfShadow;

	// 2) Keeps lightmap tint in shadow when ShadowStrength < 1.
	//    Preserves unshadowed areas when ShadowStrength < 1.
	half3 lightmapTint = bakedColorTex.rgb;
	half3 attenuatedLightmap = lightmapTint * attenuation;

	// Arbiter. Pick original lightmap value, if it is the darkest one.
	return max (min(lightmap, attenuatedLightmap), subtractedLightmap);
#else
	// Generally do min(lightmap,shadow), with "shadow" taking overall lightmap tint into account.
	half3 shadowLightmapColor = bakedColorTex.rgb * attenuation;
	half3 darkerColor = min(lightmap, shadowLightmapColor);

	// However this can darken overbright lightmaps, since "shadow color" will
	// never be overbright. So take a max of that color with attenuated lightmap color.
	return max(darkerColor, lightmap * attenuation);
#endif
}

inline void ResetUnityLight(out UnityLight outLight)
{
	outLight.color = 0;
	outLight.dir = 0;
	outLight.ndotl = 0;
}

inline void ResetUnityGI(out UnityGI outGI)
{
	ResetUnityLight(outGI.light);
	#ifdef DIRLIGHTMAP_SEPARATE
		#ifdef LIGHTMAP_ON
			ResetUnityLight(outGI.light2);
		#endif
		#ifdef DYNAMICLIGHTMAP_ON
			ResetUnityLight(outGI.light3);
		#endif
	#endif
	outGI.indirect.diffuse = 0;
	outGI.indirect.specular = 0;
}


inline UnityGI UnityGI_Base(UnityGIInput data, half occlusion, half3 normalWorld)
{
	UnityGI o_gi;
	ResetUnityGI(o_gi);


	#if !defined(LIGHTMAP_ON)
		o_gi.light = data.light;
		o_gi.light.color *= data.atten;
	#endif

	#if UNITY_SHOULD_SAMPLE_SH
		o_gi.indirect.diffuse = ShadeSHPerPixel (normalWorld, data.ambient, data.worldPos);
	#endif

	#if defined(LIGHTMAP_ON)
		// Baked lightmaps
		fixed4 bakedColorTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, data.lightmapUV.xy);
		half3 bakedColor = DecodeLightmap(bakedColorTex);

		#ifdef DIRLIGHTMAP_COMBINED
			fixed4 bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER (unity_LightmapInd, unity_Lightmap, data.lightmapUV.xy);
			o_gi.indirect.diffuse = DecodeDirectionalLightmap (bakedColor, bakedDirTex, normalWorld);

			#ifdef SHADOWS_SCREEN
				o_gi.indirect.diffuse = MixLightmapWithRealtimeAttenuation (o_gi.indirect.diffuse, data.atten, bakedColorTex, normalWorld);
			#endif // SHADOWS_SCREEN

		#elif DIRLIGHTMAP_SEPARATE
			// Left halves of both intensity and direction lightmaps store direct light; right halves - indirect.

			// Direct
			fixed4 bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd, unity_Lightmap, data.lightmapUV.xy);
			o_gi.indirect.diffuse = DecodeDirectionalSpecularLightmap (bakedColor, bakedDirTex, normalWorld, false, 0, o_gi.light);

			// Indirect
			float2 uvIndirect = data.lightmapUV.xy + float2(0.5, 0);
			bakedColor = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, uvIndirect));
			bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd, unity_Lightmap, uvIndirect);
			o_gi.indirect.diffuse += DecodeDirectionalSpecularLightmap (bakedColor, bakedDirTex, normalWorld, false, 0, o_gi.light2);

			#ifdef SHADOWS_SCREEN
				o_gi.light.color = MixLightmapWithRealtimeAttenuation(o_gi.light.color, data.atten, bakedColorTex, normalWorld);
				o_gi.light2.color = MixLightmapWithRealtimeAttenuation(o_gi.light2.color, data.atten, bakedColorTex, normalWorld);
				o_gi.indirect.diffuse = MixLightmapWithRealtimeAttenuation (o_gi.indirect.diffuse, data.atten, bakedColorTex, normalWorld);
			#endif // SHADOWS_SCREEN

		#else // not directional lightmap
			o_gi.indirect.diffuse = bakedColor;

			#ifdef SHADOWS_SCREEN
				o_gi.indirect.diffuse = MixLightmapWithRealtimeAttenuation (o_gi.indirect.diffuse, data.atten, bakedColorTex, normalWorld);
			#endif // SHADOWS_SCREEN
		#endif
	#endif

	#ifdef DYNAMICLIGHTMAP_ON
		// Dynamic lightmaps
		fixed4 realtimeColorTex = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, data.lightmapUV.zw);
		half3 realtimeColor = DecodeRealtimeLightmap (realtimeColorTex);

		#ifdef DIRLIGHTMAP_COMBINED
			half4 realtimeDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicDirectionality, unity_DynamicLightmap, data.lightmapUV.zw);
			o_gi.indirect.diffuse += DecodeDirectionalLightmap (realtimeColor, realtimeDirTex, normalWorld);

		#elif DIRLIGHTMAP_SEPARATE
			half4 realtimeDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicDirectionality, unity_DynamicLightmap, data.lightmapUV.zw);
			half4 realtimeNormalTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicNormal, unity_DynamicLightmap, data.lightmapUV.zw);
			o_gi.indirect.diffuse += DecodeDirectionalSpecularLightmap (realtimeColor, realtimeDirTex, normalWorld, true, realtimeNormalTex, o_gi.light3);
		#else
			o_gi.indirect.diffuse += realtimeColor;
		#endif
	#endif

	o_gi.indirect.diffuse *= occlusion;
	return o_gi;
}


inline half3 UnityGI_IndirectSpecular(UnityGIInput data, half occlusion, half3 normalWorld, Unity_GlossyEnvironmentData glossIn)
{
	half3 specular;

	#if UNITY_SPECCUBE_BOX_PROJECTION
		// we will tweak reflUVW in glossIn directly (as we pass it to Unity_GlossyEnvironment twice), so keep original to pass into BoxProjectedCubemapDirection
		half3 originalReflUVW = glossIn.reflUVW;
	#endif

	#if UNITY_SPECCUBE_BOX_PROJECTION
		glossIn.reflUVW = BoxProjectedCubemapDirection (originalReflUVW, data.worldPos, data.probePosition[0], data.boxMin[0], data.boxMax[0]);
	#endif

	#ifdef _GLOSSYREFLECTIONS_OFF
		specular = unity_IndirectSpecColor.rgb;
	#else
		half3 env0 = Unity_GlossyEnvironment (UNITY_PASS_TEXCUBE(unity_SpecCube0), data.probeHDR[0], glossIn);
		#if UNITY_SPECCUBE_BLENDING
			const float kBlendFactor = 0.99999;
			float blendLerp = data.boxMin[0].w;
			UNITY_BRANCH
			if (blendLerp < kBlendFactor)
			{
				#if UNITY_SPECCUBE_BOX_PROJECTION
					glossIn.reflUVW = BoxProjectedCubemapDirection (originalReflUVW, data.worldPos, data.probePosition[1], data.boxMin[1], data.boxMax[1]);
				#endif

				half3 env1 = Unity_GlossyEnvironment (UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1,unity_SpecCube0), data.probeHDR[1], glossIn);
				specular = lerp(env1, env0, blendLerp);
			}
			else
			{
				specular = env0;
			}
		#else
			specular = env0;
		#endif
	#endif

	return specular * occlusion;
}

inline UnityGI UnityGlobalIllumination (UnityGIInput data, half occlusion, half3 normalWorld)
{
	return UnityGI_Base(data, occlusion, normalWorld);
}

inline UnityGI UnityGlobalIllumination (UnityGIInput data, half occlusion, half3 normalWorld, Unity_GlossyEnvironmentData glossIn)
{
	UnityGI o_gi = UnityGI_Base(data, occlusion, normalWorld);
	o_gi.indirect.specular = UnityGI_IndirectSpecular(data, occlusion, normalWorld, glossIn);
	return o_gi;
}

//
// Old UnityGlobalIllumination signatures. Kept only for backward compatibility and will be removed soon
//

inline UnityGI UnityGlobalIllumination (UnityGIInput data, half occlusion, half oneMinusRoughness, half3 normalWorld, bool reflections)
{
	if(reflections)
	{
		Unity_GlossyEnvironmentData g;
		g.roughness		= 1 - oneMinusRoughness;
		g.reflUVW		= normalWorld;
		return UnityGlobalIllumination(data, occlusion, normalWorld, g);
	}
	else
	{
		return UnityGlobalIllumination(data, occlusion, normalWorld);
	}
}
inline UnityGI UnityGlobalIllumination (UnityGIInput data, half occlusion, half oneMinusRoughness, half3 normalWorld)
{
#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
	// No need to sample reflection probes during deferred G-buffer pass
	bool sampleReflections = false;
#else
	bool sampleReflections = true;
#endif
	return UnityGlobalIllumination (data, occlusion, oneMinusRoughness, normalWorld, sampleReflections);
}


#endif
