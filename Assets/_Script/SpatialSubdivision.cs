using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpatialSubdivision : MonoBehaviour {

	public GameObject meshRoot;
	List<Bounds> objBoundsList = new List<Bounds>();
	Bounds objBounds;

	Octree octree = new Octree();
	BVHtree bvhtree = new BVHtree();

	void Start()
	{
		var meshList = meshRoot.GetComponentsInChildren<MeshFilter>();
		if (meshList.Length <= 0) return;

		objBounds = meshList[0].mesh.bounds;
		foreach (var meshF in meshList)
		{
			var boundsList = SpatialUtils.GetMeshTriangleBoundsList(meshF.mesh);
			objBoundsList.AddRange(boundsList);
			objBounds.Encapsulate(meshF.mesh.bounds);
		}

		bvhtree.BuildTree(objBoundsList);
		octree.BuildTree(objBoundsList, objBounds);
	}

	void OnDrawGizmos()
	{
		if (!Application.isPlaying) return;
		if (!enabled) return;

		Camera camera = Camera.main;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		Gizmos.color = Color.red;
		Gizmos.DrawRay(ray);

		bvhtree.TraversalTree(delegate(BVHtree.Node node)
		{
			Gizmos.color = Color.grey;
			Gizmos.DrawWireCube(node.bounds.center, node.bounds.size);
			return true;
		});

		bvhtree.TraversalTree(delegate(BVHtree.Node node)
		{
			if (node.bounds.IntersectRay(ray))
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireCube(node.bounds.center, node.bounds.size);
				return true;
			}
			return false;
		});
	}

}
