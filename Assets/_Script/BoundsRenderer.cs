using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BoundsRenderer : MonoBehaviour {
	static public BoundsRenderer Instance { get; private set; }

	static List<int> gWireFrameIndex = new List<int>
		{	0, 1, /**/ 0, 2, /**/ 1, 3, /**/ 2, 3,
			4, 5, /**/ 4, 6, /**/ 5, 7, /**/ 6, 7,
			0, 4, /**/ 1, 5, /**/ 2, 6, /**/ 3, 7
		};

	const int vertexBuffSize = 65000;

	class MeshData
	{
		public Vector3[] vertices = new Vector3[vertexBuffSize];
		public Color[] colors = new Color[vertexBuffSize];
		public int[] indices = new int[vertexBuffSize * 3];
		public int vertexCount = 0;

		public Mesh mesh = new Mesh();
	}
	private List<MeshData> datas = new List<MeshData>();

	public Material material;

	void Awake()
	{
		Instance = this;
	}

	public void AddBounds(Bounds bounds, Color color)
	{
		MeshData meshData = null;
		for (int i = 0; i < datas.Count; ++i)
		{
			if (datas[i].vertexCount >= vertexBuffSize) continue;
			else meshData = datas[i];
		}
		if (meshData == null)
		{
			meshData = new MeshData();
			datas.Add(meshData);
		}

		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		meshData.vertices[meshData.vertexCount + 0] = min;
		meshData.vertices[meshData.vertexCount + 1].Set(min.x, min.y, max.z);
		meshData.vertices[meshData.vertexCount + 2].Set(min.x, max.y, min.z);
		meshData.vertices[meshData.vertexCount + 3].Set(min.x, max.y, max.z);
		meshData.vertices[meshData.vertexCount + 4].Set(max.x, min.y, min.z);
		meshData.vertices[meshData.vertexCount + 5].Set(max.x, min.y, max.z);
		meshData.vertices[meshData.vertexCount + 6].Set(max.x, max.y, min.z);
		meshData.vertices[meshData.vertexCount + 7] = max;

		for (int i = 0; i < 8; ++i) meshData.colors[meshData.vertexCount + i] = color;
		for (int i = 0; i < 24; ++i) meshData.indices[meshData.vertexCount * 3 + i] = gWireFrameIndex[i] + meshData.vertexCount;
		meshData.vertexCount += 8;
	}

	public void ClearBounds()
	{
		foreach (var data in datas)
		{
			data.vertexCount = 0;
			Array.Clear(data.indices, 0, data.indices.Length);
		}
	}

	public void UpdateMesh()
	{
		for (int i=0; i<datas.Count; ++i)
		{
			if (datas[i].vertexCount <= 0) continue;

			datas[i].mesh.vertices = datas[i].vertices;
			datas[i].mesh.colors = datas[i].colors;
			datas[i].mesh.SetIndices(datas[i].indices, MeshTopology.Lines, 0);
			Graphics.DrawMesh(datas[i].mesh, transform.localToWorldMatrix, material, gameObject.layer);
		}
	}
}
