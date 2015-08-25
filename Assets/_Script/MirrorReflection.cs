﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MirrorReflection : MonoBehaviour
{
	public Camera mainCamera;

	public int renderTextureSize = 256;
	public float clipPlaneOffset = 0.07f;

	private static int renderDepth = 0;
	public const int renderDepthMax = 2;

	class MirrorRender
	{
		public Camera camera = null;
		public RenderTexture target = null;
	}
	MirrorRender[] mirrorRenderers = new MirrorRender[renderDepthMax];

	void Start()
	{
		for (int i = 0; i < renderDepthMax; ++i)
		{
			if (mirrorRenderers[i] == null) mirrorRenderers[i] = new MirrorRender();
		}
	}

	void OnDestroy()
	{
		for (int i = 0; i < renderDepthMax; ++i)
		{
			if (mirrorRenderers[i] != null && mirrorRenderers[i].camera)
			{
				DestroyImmediate(mirrorRenderers[i].camera.gameObject);
			}
		}
	}

	void OnWillRenderObject()
	{
		Camera currentCamera = Camera.current;
		if (!currentCamera) return;
		if (currentCamera == mainCamera) renderDepth = 0;

		if (renderDepth >= renderDepthMax) return;

		CreateMirrorObjects(currentCamera, ref mirrorRenderers[renderDepth]);
		Camera reflectionCamera = mirrorRenderers[renderDepth].camera;

		Vector3 pos = transform.position;
		Vector3 normal = transform.up;
		float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
		Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

		Matrix4x4 reflection = Matrix4x4.zero;
		CalculateReflectionMatrix(ref reflection, reflectionPlane);
		Vector3 oldpos = currentCamera.transform.position;
		Vector3 newpos = reflection.MultiplyPoint(oldpos);
		reflectionCamera.worldToCameraMatrix = currentCamera.worldToCameraMatrix * reflection;

		Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
		Matrix4x4 projection = currentCamera.CalculateObliqueMatrix(clipPlane);
		reflectionCamera.projectionMatrix = projection;

		reflectionCamera.targetTexture = mirrorRenderers[renderDepth].target;

		renderDepth += 1;
		GL.SetRevertBackfacing(true);
		reflectionCamera.transform.position = newpos;
		Vector3 euler = currentCamera.transform.eulerAngles;
		reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
		//Debug.Log("<" + GetInstanceID() + "," + renderDepth);
		reflectionCamera.Render();
		//Debug.Log(GetInstanceID() + "," + renderDepth + ">");
		GL.SetRevertBackfacing(false);
		renderDepth -= 1;

		SetReflectTexture(mirrorRenderers[renderDepth].target);
	}


	void SetReflectTexture(RenderTexture renderTexture)
	{
		var rend = GetComponent<Renderer>();
		if (!rend || !rend.sharedMaterial) return;

		Material[] materials = rend.sharedMaterials;
		foreach (Material mat in materials)
		{
			if (mat.HasProperty("_ReflectionTex"))
				mat.SetTexture("_ReflectionTex", renderTexture);
		}
	}

	private void UpdateCameraModes(Camera src, Camera dest)
	{
		if (dest == null)
			return;
		// set camera to clear the same way as current camera
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox sky = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox mysky = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!sky || !sky.material)
			{
				mysky.enabled = false;
			}
			else
			{
				mysky.enabled = true;
				mysky.material = sky.material;
			}
		}

		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}

	private void CreateMirrorObjects(Camera currentCamera, ref MirrorRender reflectionRender)
	{
		if (reflectionRender.target == null)
		{
			reflectionRender.target = new RenderTexture(renderTextureSize, renderTextureSize, 16);
			reflectionRender.target.name = GetInstanceID() + "," + renderDepth;
			reflectionRender.target.isPowerOfTwo = true;
			reflectionRender.target.hideFlags = HideFlags.DontSave;
		}

		if (reflectionRender.camera != null)
		{
			reflectionRender.camera.enabled = false;
			reflectionRender.camera.transform.position = transform.position;
			reflectionRender.camera.transform.rotation = transform.rotation;
		}
		else
		{
			GameObject go = new GameObject(GetInstanceID() + "," + renderDepth, typeof(Camera), typeof(Skybox));
			reflectionRender.camera = go.camera;
			reflectionRender.camera.enabled = false;
			reflectionRender.camera.transform.position = transform.position;
			reflectionRender.camera.transform.rotation = transform.rotation;
			reflectionRender.camera.gameObject.AddComponent("FlareLayer");
			go.hideFlags = HideFlags.DontSave;
		}

		UpdateCameraModes(currentCamera, reflectionRender.camera);
	}

	// Extended sign: returns -1, 0 or 1 based on sign of a
	private static float sgn(float a)
	{
		if (a > 0.0f) return 1.0f;
		if (a < 0.0f) return -1.0f;
		return 0.0f;
	}

	// Given position/normal of the plane, calculates plane in camera space.
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * clipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint(offsetPos);
		Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
	}

	// Calculates reflection matrix around the given plane
	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
		reflectionMat.m01 = (-2F * plane[0] * plane[1]);
		reflectionMat.m02 = (-2F * plane[0] * plane[2]);
		reflectionMat.m03 = (-2F * plane[3] * plane[0]);

		reflectionMat.m10 = (-2F * plane[1] * plane[0]);
		reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
		reflectionMat.m12 = (-2F * plane[1] * plane[2]);
		reflectionMat.m13 = (-2F * plane[3] * plane[1]);

		reflectionMat.m20 = (-2F * plane[2] * plane[0]);
		reflectionMat.m21 = (-2F * plane[2] * plane[1]);
		reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
		reflectionMat.m23 = (-2F * plane[3] * plane[2]);

		reflectionMat.m30 = 0F;
		reflectionMat.m31 = 0F;
		reflectionMat.m32 = 0F;
		reflectionMat.m33 = 1F;
	}
}