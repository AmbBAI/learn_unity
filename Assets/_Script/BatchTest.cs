using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BatchTest : MonoBehaviour {

	public GameObject obj;
	List<GameObject> objPool = new List<GameObject>();
	int vertexCount = 0;

	void Start()
	{
		MeshFilter mf = obj.GetComponent<MeshFilter>();
		Mesh m = mf.sharedMesh;
		vertexCount = m.vertexCount;
	}

	void OnGUI()
	{
		Rect rect = new Rect(0, 0, 200, 20);
		GUI.Label(rect, "Mesh Vertex Count: " + vertexCount);
		rect.Set(rect.xMin, rect.yMin + 20, rect.width, rect.height);
		GUI.Label(rect, "Mesh Count: " + objPool.Count);
		rect.Set(rect.xMin, rect.yMin + 20, rect.width, rect.height);
		GUI.Label(rect, "Total Vertex Count: " + vertexCount * objPool.Count);
		rect.Set(rect.xMin, rect.yMin + 20, rect.width, rect.height);
		if (GUI.Button(rect, "Add Obj"))
		{
			Object newObj = GameObject.Instantiate(obj);
			objPool.Add(newObj as GameObject);
		}
		rect.Set(rect.xMin, rect.yMin + 20, rect.width, rect.height);
	}
}
