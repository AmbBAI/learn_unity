using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

[System.Serializable]
public class RendererLightmapInfo
{
	public int lightmapIndex;
	public Vector4 lightmapScaleOffset;
}

public class LightmapInfo
{
	[JsonIgnore]
	public string key;
	[JsonIgnore]
	public int lightmapOffset;
	public int lightmapCount;
	public Dictionary<string, RendererLightmapInfo> objects = new Dictionary<string, RendererLightmapInfo>();
}

public class LightmapTool : MonoBehaviour
{
	const string lightmapInfoPath = "LightmapData/";
	const string lightmapNearName = "Lightmap-{0}_comp_dir";
	const string lightmapFarName = "Lightmap-{0}_comp_light";

	public LightmapInfo LoadLightmapInfo(string sceneKey)
	{
		var asset = Resources.Load<TextAsset>(lightmapInfoPath + sceneKey);
		string info = asset.text;
		try
		{
			Debug.Log(info);
			ParseUtils.RegisterJsonMapperImporter();
			var lightmapInfo = JsonMapper.ToObject<LightmapInfo>(info);
			if (lightmapInfo == null)
			{
				throw new System.Exception("lightmapInfo format error!");
			}
			lightmapInfo.key = sceneKey;
			return lightmapInfo;
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
		}
		return null;
	}

	public void LoadLightmaps(ref List<LightmapInfo> list)
	{
		List<LightmapData> lightmapDataList = new List<LightmapData>();
		Dictionary<string, int> lightmapDataOffset = new Dictionary<string, int>();
		for (int i = 0; i < list.Count; ++i)
		{
			if (lightmapDataOffset.ContainsKey(list[i].key))
			{
				list[i].lightmapOffset = lightmapDataOffset[list[i].key];
			}
			else
			{
				list[i].lightmapOffset = lightmapDataList.Count;
				var lightmapDir = string.Format(lightmapInfoPath + list[i].key + "/");
				for (int j = 0; j < list[i].lightmapCount; ++j)
				{
					LightmapData lightmapData = new LightmapData();
					lightmapData.lightmapNear = Resources.Load<Texture2D>(string.Format(lightmapDir + lightmapNearName, j));
					lightmapData.lightmapFar = Resources.Load<Texture2D>(string.Format(lightmapDir + lightmapFarName, j));
					Debug.Log(string.Format(lightmapDir + lightmapNearName, j) + "==>" + lightmapData.lightmapNear);
					Debug.Log(string.Format(lightmapDir + lightmapFarName, j) + "==>" + lightmapData.lightmapFar);
					lightmapDataList.Add(lightmapData);
				}
				lightmapDataOffset[list[i].key] = list[i].lightmapOffset;
			}
		}
		LightmapSettings.lightmaps = lightmapDataList.ToArray();
	}

	public void AssetRendererLightmap(GameObject root, LightmapInfo lightmapInfo)
	{
		Renderer[] renderer;
		if (root == null) renderer = FindObjectsOfType<MeshRenderer>();
		else renderer = root.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < renderer.Length; ++i)
		{
			RendererLightmapInfo info = null;
			if (lightmapInfo.objects.TryGetValue(renderer[i].name, out info))
			{
				renderer[i].lightmapIndex = (info.lightmapIndex + lightmapInfo.lightmapOffset);
				renderer[i].lightmapScaleOffset = info.lightmapScaleOffset;
			}
		}
		//Terrain[] terrain = root.GetComponentsInChildren<Terrain>();
		//for (int i = 0; i < terrain.Length; ++i)
		//{
		//    RendererLightmapInfo info = null;
		//    if (lightmapInfo.objects.TryGetValue(terrain[i].name, out info))
		//    {
		//        terrain[i].lightmapIndex = (info.lightmapIndex + lightmapInfo.lightmapOffset);
		//        terrain[i].lightmapScaleOffset = info.lightmapScaleOffset;
		//    }
		//}
	}

#if UNITY_EDITOR
	public string key = string.Empty;

	[ContextMenu("Save Lightmap Info")]
	public void SaveLightMapInfo()
	{
		LightmapInfo lightmapInfo = new LightmapInfo();

		Renderer[] renderer;
		renderer = FindObjectsOfType<MeshRenderer>();
		//renderer = GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < renderer.Length; ++i)
		{
			if (renderer[i].lightmapIndex < 0) continue;

			RendererLightmapInfo info = new RendererLightmapInfo();
			info.lightmapIndex = renderer[i].lightmapIndex;
			info.lightmapScaleOffset = renderer[i].lightmapScaleOffset;
			try
			{
				lightmapInfo.objects.Add(renderer[i].name, info);
			}
			catch (System.Exception e)
			{
				Debug.LogWarning(renderer[i].name);
				Debug.LogException(e);
			}
		}
		//Terrain[] terrain = GetComponentsInChildren<Terrain>();
		//for (int i = 0; i < terrain.Length; ++i)
		//{
		//    if (terrain[i].lightmapIndex < 0) continue;

		//    RendererLightmapInfo info = new RendererLightmapInfo();
		//    info.lightmapIndex = terrain[i].lightmapIndex;
		//    info.lightmapScaleOffset = terrain[i].lightmapScaleOffset;
		//    try
		//    {
		//        lightmapInfo.objects.Add(terrain[i].name, info);
		//    }
		//    catch (System.Exception e)
		//    {
		//        Debug.LogWarning(terrain[i].name);
		//        Debug.LogException(e);
		//    }
		//}
		lightmapInfo.lightmapCount = LightmapSettings.lightmaps.Length;

		ParseUtils.RegisterJsonMapperExporter();
		string json = JsonMapper.ToJson(lightmapInfo);
		FileUtils.SaveStringToFile("Assets/Resources/" + lightmapInfoPath + key + ".json", json);
	}

	[ContextMenu("Load And Test LightmapInfo")]
	public void LoadAndTestLightmapInfo()
	{
		LightmapInfo lightmapInfo = LoadLightmapInfo(key);
		if (lightmapInfo == null) return;

		List<LightmapInfo> lightmapInfoList = new List<LightmapInfo>() { lightmapInfo };
		LoadLightmaps(ref lightmapInfoList);

		AssetRendererLightmap(gameObject, lightmapInfo);
	}
#endif

}
