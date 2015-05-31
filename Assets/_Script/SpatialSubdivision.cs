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
			var boundsList = AabbUtils.GetAabbList(meshF.mesh);
			objBoundsList.AddRange(boundsList);
			objBounds.Encapsulate(meshF.mesh.bounds);
		}

		bvhtree.BuildTree(objBoundsList);
	}

	void Update()
	{
		BoundsRenderer br = BoundsRenderer.Instance;
		br.ClearBounds();

		Camera camera = Camera.main;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);

		bvhtree.TraversalTree(delegate(BVHtree.BVHtreeNode node)
		{
			//render.AddBounds(node.bounds, Color.grey);
			//return true;

			if (node.bounds.IntersectRay(ray))
			{
				br.AddBounds(node.bounds, Color.green);
				return true;
			}
			return false;
		});

		br.UpdateMesh();
	}

}
