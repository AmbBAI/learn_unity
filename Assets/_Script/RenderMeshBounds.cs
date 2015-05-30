using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderMeshBounds : MonoBehaviour {

	Octree octree = new Octree();

	void Start()
	{

		var meshFs = GetComponentsInChildren<MeshFilter>();
		List<Bounds> all = new List<Bounds>();
		Bounds bounds = new Bounds();
		foreach (var meshF in meshFs)
		{
			var boundsList = AabbUtils.GetAabbList(meshF.mesh);
			all.AddRange(boundsList);
			bounds.Encapsulate(meshF.mesh.bounds);
		}

		octree.BuildTree(all, bounds);
	}

	void Update()
	{
		BoundsRenderer r = BoundsRenderer.Instance;
		r.ClearBounds();
		octree.DrawTree();
		r.UpdateMesh();
	}
}
