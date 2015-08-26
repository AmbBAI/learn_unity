using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MirrorReflection : MonoBehaviour
{
	public Camera mainCamera;

	public int renderTextureSize = 256;
	public float clipPlaneOffset = 0.07f;

	private static int renderDepth = 0;
	public const int renderDepthMax = 3;

	Camera[] renderCameraPool = new Camera[renderDepthMax];

	void Start()
	{
		for (int i=0; i<renderDepthMax; ++i)
		{
			renderCameraPool[i] = CreateMirrorCamera();
		}
	}

	void OnDestroy()
	{
		for (int i = 0; i < renderDepthMax; ++i)
		{
			if (renderCameraPool[i] != null)
			{
				DestroyImmediate(renderCameraPool[i].gameObject);
			}
		}
	}

	Camera CreateMirrorCamera()
	{
		GameObject go = new GameObject("__MirrorCamera", typeof(Camera), typeof(Skybox));
		go.hideFlags = HideFlags.HideAndDontSave;
		var cam = go.GetComponent<Camera>();
		cam.enabled = false;
		cam.targetTexture = new RenderTexture(renderTextureSize, renderTextureSize, 24);
		cam.targetTexture.isPowerOfTwo = true;
		return cam;
	}

	void OnWillRenderObject()
	{
		Camera currentCamera = Camera.current;
		if (!currentCamera) return;

		if (renderDepth >= renderDepthMax)
		{
			SetReflectTexture(null);
			return;
		}

		Camera reflectionCamera = renderCameraPool[renderDepth];
		UpdateCameraModes(currentCamera, reflectionCamera);

		Vector3 position = transform.position;
		Vector3 normal = transform.up;
		float dis = -Vector3.Dot(normal, position) - clipPlaneOffset;
		Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, dis);

		Matrix4x4 reflection = Matrix4x4.zero;
		CalculateReflectionMatrix(ref reflection, reflectionPlane);
		reflectionCamera.worldToCameraMatrix = currentCamera.worldToCameraMatrix * reflection;

		Vector4 clipPlane = CameraSpacePlane(reflectionCamera, position, normal, 1f);
		Matrix4x4 projection = currentCamera.CalculateObliqueMatrix(clipPlane);
		reflectionCamera.projectionMatrix = projection;

		renderDepth += 1;
		GL.SetRevertBackfacing((renderDepth & 1) == 1);
		reflectionCamera.Render();
		GL.SetRevertBackfacing((renderDepth & 1) == 0);
		renderDepth -= 1;

		SetReflectTexture(reflectionCamera.targetTexture);
	}

	void OnRenderObject()
	{
		if (renderDepth > 0)
		{
			SetReflectTexture(renderCameraPool[renderDepth - 1].targetTexture);
		}
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
		if (dest == null) return;
		
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

	private Vector4 CameraSpacePlane(Camera cam, Vector3 position, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = position + normal * clipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cposition = m.MultiplyPoint(offsetPos);
		Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cposition, cnormal));
	}

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