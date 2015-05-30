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
		public MeshFilter meshFilter = null;
	}
	private List<MeshData> datas = new List<MeshData>();

	private GameObject boundsRendererPrefab = null;

	void Awake()
	{
		Instance = this;
		boundsRendererPrefab = Resources.Load<GameObject>("BoundsRenderer");
	}

	public void AddBounds(Bounds bounds, Color color)
	{
		MeshData meshData = null;
		foreach (var data in datas)
		{
			if (data.vertexCount >= vertexBuffSize) continue;
			else meshData = data;
		}
		if (meshData == null)
		{
			meshData = new MeshData();
			datas.Add(meshData);
		}

		List<Vector3> vertex = new List<Vector3>();
		meshData.vertices[meshData.vertexCount + 0] = bounds.min;
		meshData.vertices[meshData.vertexCount + 1].Set(bounds.min.x, bounds.min.y, bounds.max.z);
		meshData.vertices[meshData.vertexCount + 2].Set(bounds.min.x, bounds.max.y, bounds.min.z);
		meshData.vertices[meshData.vertexCount + 3].Set(bounds.min.x, bounds.max.y, bounds.max.z);
		meshData.vertices[meshData.vertexCount + 4].Set(bounds.max.x, bounds.min.y, bounds.min.z);
		meshData.vertices[meshData.vertexCount + 5].Set(bounds.max.x, bounds.min.y, bounds.max.z);
		meshData.vertices[meshData.vertexCount + 6].Set(bounds.max.x, bounds.max.y, bounds.min.z);
		meshData.vertices[meshData.vertexCount + 7] = bounds.max;

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
			if (datas[i].meshFilter == null)
			{
				GameObject obj = GameObject.Instantiate(boundsRendererPrefab) as GameObject;
				obj.SetActive(true);
				obj.transform.parent = transform;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localRotation = Quaternion.identity;
				obj.transform.localScale = Vector3.one;
				datas[i].meshFilter = obj.GetComponent<MeshFilter>();
				datas[i].meshFilter.mesh = datas[i].mesh;
			}

			if (datas[i].vertexCount <= 0)
			{
				datas[i].meshFilter.gameObject.SetActive(false);
				continue;
			}

			datas[i].mesh.vertices = datas[i].vertices;
			datas[i].mesh.colors = datas[i].colors;
			datas[i].mesh.SetIndices(datas[i].indices, MeshTopology.Lines, 0);
			datas[i].meshFilter.gameObject.SetActive(true);
		}
	}
}
