using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoundsRenderer : MonoBehaviour {
	static public BoundsRenderer Instance { get; private set; }

	static List<int> gWireFrameIndex = new List<int>
		{	0, 1, /**/ 0, 2, /**/ 1, 3, /**/ 2, 3,
			4, 5, /**/ 4, 6, /**/ 5, 7, /**/ 6, 7,
			0, 4, /**/ 1, 5, /**/ 2, 6, /**/ 3, 7
		};

	private List<MeshFilter> meshFilters = new List<MeshFilter>();
	class MeshData
	{
		public List<Vector3> vertices = new List<Vector3>();
		public List<int> indices = new List<int>();
		public List<Color> colors = new List<Color>();
		public Mesh mesh = new Mesh();
	}
	private List<MeshData> datas = new List<MeshData>();

	private GameObject boundsRendererPrefab = null;

	void Awake()
	{
		Instance = this;
		boundsRendererPrefab = Resources.Load<GameObject>("BoundsRenderer");
	}

	public void DrawBounds(Bounds bounds, Color color)
	{
		MeshData meshData = null;
		foreach (var data in datas)
		{
			if (data.vertices.Count >= (65000 / 8 * 8)) continue;
			else meshData = data;
		}
		if (meshData == null)
		{
			meshData = new MeshData();
			datas.Add(meshData);
		}

		List<Vector3> vertex = new List<Vector3>();
		vertex.Add(bounds.min);
		vertex.Add(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
		vertex.Add(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z));
		vertex.Add(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
		vertex.Add(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
		vertex.Add(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
		vertex.Add(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z));
		vertex.Add(bounds.max);

		int offset = meshData.vertices.Count;
		meshData.vertices.AddRange(vertex);
		for (int i = 0; i < 8; ++i) meshData.colors.Add(color);
		for (int i = 0; i < 24; ++i) meshData.indices.Add(gWireFrameIndex[i] + offset);
	}

	public void ClearBounds()
	{
		datas.Clear();
		UpdateMesh();
	}

	public void UpdateMesh()
	{
		for (int i=0; i<Mathf.Max(meshFilters.Count, datas.Count); ++i)
		{
			if (i < datas.Count)
			{
				datas[i].mesh.vertices = datas[i].vertices.ToArray();
				datas[i].mesh.colors = datas[i].colors.ToArray();
				datas[i].mesh.SetIndices(datas[i].indices.ToArray(), MeshTopology.Lines, 0);
			}

			if (i < Mathf.Min(datas.Count, meshFilters.Count))
			{
				meshFilters[i].gameObject.SetActive(true);
				meshFilters[i].mesh = datas[i].mesh;
			}
			else if (i < datas.Count)
			{
				GameObject obj = GameObject.Instantiate(boundsRendererPrefab) as GameObject;
				obj.SetActive(true);
				obj.transform.parent = transform;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localRotation = Quaternion.identity;
				obj.transform.localScale = Vector3.one;
				MeshFilter meshFilter = obj.GetComponent<MeshFilter>();

				meshFilter.mesh = datas[i].mesh;
				meshFilters.Add(meshFilter);
			}
			else
			{
				meshFilters[i].gameObject.SetActive(false);
			}
		}
	}
}
