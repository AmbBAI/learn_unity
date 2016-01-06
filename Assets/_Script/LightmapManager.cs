using UnityEngine;
using System.Collections.Generic;

public class LightmapManager : MonoBehaviour {

	public LightmapTool lightmapTool;
    public GameObject root1;
	public string lightmap1;
	public GameObject root2;
	public string lightmap2;

	void Start()
	{
		Load();
	}

	[ContextMenu("Load")]
	public void Load()
	{
		var lm1 = lightmapTool.LoadLightmapInfo(lightmap1);
		var lm2 = lightmapTool.LoadLightmapInfo(lightmap2);
		var list = new List<LightmapInfo>() { lm1, lm2 };
		lightmapTool.LoadLightmaps(ref list);
		lightmapTool.AssetRendererLightmap(root1, lm1);
		lightmapTool.AssetRendererLightmap(root2, lm2);
	}

	[ContextMenu("Test")]
	public void Test()
	{
		var lm1 = lightmapTool.LoadLightmapInfo(lightmap1);
		var list = new List<LightmapInfo>() { lm1 };
		lightmapTool.AssetRendererLightmap(root1, lm1);
	}

	[ContextMenu("Test2")]
	public void Test2()
	{
		var lm1 = lightmapTool.LoadLightmapInfo(lightmap1);
		var list = new List<LightmapInfo>() { lm1 };
		lightmapTool.LoadLightmaps(ref list);
		lightmapTool.AssetRendererLightmap(root1, lm1);
	}

}
