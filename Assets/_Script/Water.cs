using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Water : MonoBehaviour {

	public List<int> storeVertex;

	MeshRenderer meshRender;
	MeshFilter meshFilter;
	Mesh mesh;

	Color[] colors = null;
	Vector2[] uvs = null;

	void Start () {
		meshRender = GetComponent<MeshRenderer>();
		meshFilter = GetComponent<MeshFilter>();
		mesh = meshFilter.sharedMesh;

		InitMeshData();
	}

	void InitMeshData()
	{
		int count = mesh.vertexCount;
		if (colors == null) colors = new Color[count];
		if (uvs == null) uvs = new Vector2[count];

		for (int i = 0; i < count; ++i)
		{
			colors[i] = new Color(0f, Random.Range(0f, 1f), Random.Range(0f, 1f), 0.1f);
			colors[i].a = ((mesh.vertices[i].x + mesh.vertices[i].z) * 0.25f) + 0.01f;
			uvs[i] = new Vector2(1f, 1f) * Random.Range(0f, 1f);
		}
		mesh.colors = colors;
		mesh.uv = uvs;
	}
	
}
