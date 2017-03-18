using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RecursiveCamera : MonoBehaviour
{
	public bool isInRendering = false;
	public int renderTextureSize = 512;
	public RenderTexture renderTexture = null;
	public List<Camera> cameraList = new List<Camera>();

	void Update()
	{
		isInRendering = false;
	}

	void OnWillRenderObject()
	{
		if (isInRendering) return;
		isInRendering = true;

		Camera currentCamera = Camera.current;
		if (!currentCamera) return;

		if (renderTexture == null)
		{
			renderTexture = new RenderTexture(renderTextureSize, renderTextureSize, 24);
		}

		for (int i = 0; i < cameraList.Count; ++i)
		{
			if (i == 0) SetReflectTexture(null);
			else SetReflectTexture(cameraList[i - 1].targetTexture);

			if (cameraList[i].targetTexture == null)
			{
				cameraList[i].targetTexture = new RenderTexture(renderTextureSize, renderTextureSize, 24);
			}
			cameraList[i].Render();
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
}
