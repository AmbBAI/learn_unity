using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BatchTest : MonoBehaviour {

	public GameObject obj;
	public Transform root;
	List<GameObject> objPool = new List<GameObject>();
	int vertexCount = 0;
	int triangleCount = 0;

	void Start()
	{
		MeshFilter mf = obj.GetComponent<MeshFilter>();
		Mesh m = mf.sharedMesh;
		vertexCount = m.vertexCount;
		triangleCount = m.triangles.Length / 3;
	}

	void OnGUI()
	{
		Rect rect = new Rect(0, 0, 200, 20);
		GUI.Label(rect, "Mesh Vertex Count: " + vertexCount);
		rect.Set(rect.xMin, rect.yMin + 20, rect.width, rect.height);
		GUI.Label(rect, "Mesh Triangle Count: " + triangleCount);
		rect.Set(rect.xMin, rect.yMin + 20, rect.width, rect.height);
		GUI.Label(rect, "Mesh Count: " + objPool.Count);
		rect.Set(rect.xMin, rect.yMin + 20, rect.width, rect.height);
		GUI.Label(rect, "Total Vertex Count: " + vertexCount * objPool.Count);
		rect.Set(rect.xMin, rect.yMin + 20, rect.width, rect.height);
		GUI.Label(rect, "Total Triangle Count: " + triangleCount * objPool.Count);
		rect.Set(rect.xMin, rect.yMin + 20, rect.width, rect.height);
		if (GUI.Button(rect, "Add Obj"))
		{
			GameObject newObj = GameObject.Instantiate(obj) as GameObject;
			newObj.transform.parent = root;
			newObj.transform.localPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(0f, 1f));
			objPool.Add(newObj);
		}
		rect.Set(rect.xMin, rect.yMin + 20, rect.width, rect.height);
		if (GUI.Button(rect, "Run Static Batch"))
		{
			StaticBatchingUtility.Combine(root.gameObject);
		}
		rect.Set(rect.xMin, rect.yMin + 20, rect.width, rect.height);
	}
}
