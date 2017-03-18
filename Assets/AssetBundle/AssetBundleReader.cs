using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleReader : MonoBehaviour
{
	public static AssetBundleReader Instance { set; get; }

	public bool EditorDirectLoad = true;

	public int version = 0;
	private string localRoot = "";
	private string bundleExt = "assetbundle";
	private string packPath = "Assets/Resources/";

	Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>();

	public delegate IEnumerator AssetBundleLoadDelegate(AssetBundle assetBundle);
	public delegate void AssetLoadDelegate(Object assetPrefeb);

	void Awake()
	{
		Instance = this;

		localRoot =
#if UNITY_EDITOR
		"file://" + Application.streamingAssetsPath;
#elif UNITY_ANDROID
		Application.streamingAssetsPath;
#else
		"file://" + Application.streamingAssetsPath;
#endif
	}

	public IEnumerator LoadAssetBundle(string package)
	{
		while (!Caching.ready)
			yield return null;

		string loadPath = string.Format("{0}/{1}.{2}", localRoot, package, bundleExt);
		Debug.Log(loadPath);

		using (WWW www = WWW.LoadFromCacheOrDownload(loadPath, version))
		{
			yield return www;

			if (www.error != null)
			{
				Debug.Log(www.error);
				yield break;
			}

			AssetBundle assetBundle = www.assetBundle;
			if (assetBundle != null)
			{
				if (assetBundleDict.ContainsKey(package))
					assetBundle.Unload(true);
				else
					assetBundleDict[package] = assetBundle;
			}

			www.Dispose();
		}
	}

	public IEnumerator LoadAssetBundle(string package, AssetBundleLoadDelegate loadCallBack)
	{
		if (!assetBundleDict.ContainsKey(package))
			yield return StartCoroutine(LoadAssetBundle(package));

		AssetBundle assetBundle = null;
		assetBundleDict.TryGetValue(package, out assetBundle);
		if (assetBundle == null) yield break;

		yield return StartCoroutine(loadCallBack(assetBundle));
	}

	public IEnumerator LoadAssetAsync(string package, string asset, System.Type type, AssetLoadDelegate loadCallBack)
	{
#if UNITY_EDITOR
		if (EditorDirectLoad)
		{
			string assetPath = string.Format("{0}{1}/{2}.prefab", packPath, package, asset);
			Debug.Log(assetPath);
			Object assetObject = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, type);
			loadCallBack(assetObject as GameObject);
			yield break;
		}
#endif
		if (!assetBundleDict.ContainsKey(package))
			yield return StartCoroutine(LoadAssetBundle(package));

		AssetBundle assetBundle = null;
		assetBundleDict.TryGetValue(package, out assetBundle);
		if (assetBundle == null) yield break;

		AssetBundleRequest asyncRequest = assetBundle.LoadAssetAsync(asset, type);
		yield return asyncRequest;

		loadCallBack(asyncRequest.asset);
	}

	public void UnloadAssetBundle(string package)
	{
		AssetBundle assetBundle = null;
		assetBundleDict.TryGetValue(package, out assetBundle);
		if (assetBundle == null) return;
		assetBundle.Unload(false);
		assetBundleDict.Remove(package);
	}

	public void UnloadAllAssetBundle()
	{
		foreach (KeyValuePair<string, AssetBundle> itor in assetBundleDict)
		{
			(itor.Value).Unload(false);
		}
		assetBundleDict.Clear();
	}

	public IEnumerator LoadAssetBundleNoCache(string package, AssetBundleLoadDelegate loadCallBack)
	{
		while (!Caching.ready)
			yield return null;

		string loadPath = string.Format("{0}/{1}.{2}", localRoot, package, bundleExt);
		Debug.Log(loadPath);

		using (WWW www = WWW.LoadFromCacheOrDownload(loadPath, version))
		{
			yield return www;

			AssetBundle assetBundle = www.assetBundle;

			yield return StartCoroutine(loadCallBack(assetBundle));

			assetBundle.Unload(false);

			www.Dispose();
		}
		yield return null;
	}

}
