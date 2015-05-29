using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BoundsRenderer : MonoBehaviour {
	static public BoundsRenderer Instance { get; private set; }

	private MeshFilter meshFilter;
	private Mesh mesh = null;
	private MeshRenderer meshRenderer;

	private List<Vector3> vertices = new List<Vector3>();
	private List<int> indices = new List<int>();
	private List<Color> colors = new List<Color>();

	static List<int> gWireFrameIndex = new List<int>
		{	0, 1, /**/ 0, 2, /**/ 1, 3, /**/ 2, 3,
			4, 5, /**/ 4, 6, /**/ 5, 7, /**/ 6, 7,
			0, 4, /**/ 1, 5, /**/ 2, 6, /**/ 3, 7
		};

	void Awake()
	{
		Instance = this;

		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();

		mesh = new Mesh();
		meshFilter.mesh = mesh;
	}

	public void DrawBounds(Bounds bounds, Color color)
	{
		List<Vector3> vertex = new List<Vector3>();
		vertex.Add(bounds.min);
		vertex.Add(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
		vertex.Add(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z));
		vertex.Add(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
		vertex.Add(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
		vertex.Add(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
		vertex.Add(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z));
		vertex.Add(bounds.max);

		int offset = vertices.Count;
		vertices.AddRange(vertex);
		for (int i = 0; i < 8; ++i) colors.Add(color);
		for (int i = 0; i < 24; ++i) indices.Add(gWireFrameIndex[i] + offset);

		UpdateMesh();
	}

	public void ClearBounds()
	{
		vertices.Clear();
		colors.Clear();
		indices.Clear();

		UpdateMesh();
	}

	public void UpdateMesh()
	{
		mesh.vertices = vertices.ToArray();
		mesh.colors = colors.ToArray();
		mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
	}
}
