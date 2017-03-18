using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleTest : MonoBehaviour {

	List<GameObject> objectList = new List<GameObject>();

	int totalTask = 0;
	int loadTask = 0;
	float asyncTaskStartTime = 0;

	AssetBundleReader reader = null;

	void Start ()
	{
		reader = AssetBundleReader.Instance;
	}

	void OnGUI ()
	{
		float autoY = 0f;
		float buttonH = 50f;
		if (GUI.Button(new Rect(0, autoY, 200, buttonH), "LoadResources"))
		{
			LoadResources();
		} autoY += buttonH;

		if (GUI.Button(new Rect(0, autoY, 200, buttonH), "LoadAssetsAsync"))
		{
			LoadAssetsAsync();
		} autoY += buttonH;

		if (GUI.Button(new Rect(0, autoY, 200, buttonH), "ClearAssetBundleBuffer"))
		{
			reader.UnloadAllAssetBundle();
		} autoY += buttonH;

		if (GUI.Button(new Rect(0, autoY, 200, buttonH), "ClearObjects"))
		{
			ClearObjects();
		} autoY += buttonH;

		if (GUI.Button(new Rect(0, autoY, 200, buttonH), "Reset"))
		{
			ClearObjects();
			Resources.UnloadUnusedAssets();
			reader.UnloadAllAssetBundle();
		} autoY += buttonH;
	}

	void LoadResources()
	{
		float startTime = Time.realtimeSinceStartup;

		Object preAtomBall = Resources.Load("AssetBundleTest/obj1");
		objectList.Add(Instantiate(preAtomBall) as GameObject);

		Object preSpikeBall = Resources.Load("AssetBundleTest/obj2");
		objectList.Add(Instantiate(preSpikeBall) as GameObject);

		Object prePlayer = Resources.Load("AssetBundleTest/obj3");
		objectList.Add(Instantiate(prePlayer) as GameObject);

		float endTime = Time.realtimeSinceStartup;
		Debug.Log(string.Format("LoadResources Cost: {0}", endTime - startTime));
	}

	void OnAsyncTaskStart()
	{
		totalTask = 3;
		loadTask = 0;
		asyncTaskStartTime = Time.realtimeSinceStartup;
	}

	void OnLoadDone()
	{
		loadTask += 1;
		if (loadTask >= totalTask)
		{
			float endTime = Time.realtimeSinceStartup;
			Debug.Log(string.Format("LoadAssetBundle Cost: {0}", endTime - asyncTaskStartTime));
		}
	}

	void LoadAssetsAsync()
	{
		OnAsyncTaskStart();
		StartCoroutine(reader.LoadAssetAsync("assetbundle_test", "obj1", typeof(GameObject),
			delegate(Object asset)
			{
				objectList.Add(Instantiate(asset) as GameObject);
				OnLoadDone();
			}));

		StartCoroutine(reader.LoadAssetAsync("assetbundle_test", "obj2", typeof(GameObject),
			delegate(Object asset)
			{
				objectList.Add(Instantiate(asset) as GameObject);
				OnLoadDone();
			}));

		StartCoroutine(reader.LoadAssetAsync("assetbundle_test", "obj3", typeof(GameObject),
			delegate(Object asset)
			{
				objectList.Add(Instantiate(asset) as GameObject);
				OnLoadDone();
			}));
	}

	void ClearObjects()
	{
		foreach (GameObject obj in objectList)
		{
			Destroy(obj);
		}
		objectList.Clear();
	}
}
