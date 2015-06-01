using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpatialSubdivision : MonoBehaviour {

	public GameObject meshRoot;
	List<Bounds> objBoundsList = new List<Bounds>();
	Bounds objBounds;

	List<KDtreeObject> objs;
	Vector3 point;
	public float moveScale = 10f;

	Octree octree = new Octree();
	BVHtree bvhtree = new BVHtree();
	KDtree<KDtreeObject> kdtree = new KDtree<KDtreeObject>();

	void Start()
	{
		Bounds bounds = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(1000f, 1000f, 0f));
		objs = SpatialUtils.CreateKDTreeObjectPool(bounds, 10000);

		kdtree.BuildTree(objs);
		point = Vector3.zero;

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
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.W)) point += Vector3.forward * (moveScale * Time.deltaTime);
		if (Input.GetKey(KeyCode.A)) point += Vector3.left * (moveScale * Time.deltaTime);
		if (Input.GetKey(KeyCode.S)) point += Vector3.back * (moveScale * Time.deltaTime);
		if (Input.GetKey(KeyCode.D)) point += Vector3.right * (moveScale * Time.deltaTime);
		if (Input.GetKey(KeyCode.Q)) point += Vector3.up * (moveScale * Time.deltaTime);
		if (Input.GetKey(KeyCode.E)) point += Vector3.down * (moveScale * Time.deltaTime);

		//BoundsRenderer br = BoundsRenderer.Instance;
		//br.ClearBounds();

		//Camera camera = Camera.main;
		//Ray ray = camera.ScreenPointToRay(Input.mousePosition);

		//bvhtree.TraversalTree(delegate(BVHtree.Node node)
		//{
		//	br.AddBounds(node.bounds, Color.grey);
		//	return true;
		//});

		//bvhtree.TraversalTree(delegate(BVHtree.Node node)
		//{
		//	if (node.bounds.IntersectRay(ray))
		//	{
		//		br.AddBounds(node.bounds, Color.green);
		//		return true;
		//	}
		//	return false;
		//});

		//var ret = kdtree.SearchNode(point);
		//for (int i = 0; i < objs.Count; ++i)
		//{
		//	if (ret.obj == objs[i])
		//	{
		//		br.AddBounds(new Bounds(objs[i].position, Vector3.one), Color.green);
		//	}
		//	else
		//	{
		//		br.AddBounds(new Bounds(objs[i].position, Vector3.one), Color.grey);
		//	}
		//}

		//br.AddBounds(new Bounds(point, Vector3.one), Color.red);

		//br.UpdateMesh();


	}

	void OnDrawGizmos()
	{
		if (!Application.isPlaying) return;

		var ret = kdtree.SearchNode(point);
		for (int i = 0; i < objs.Count; ++i)
		{
			if (ret.obj == objs[i])
			{
				Gizmos.color = Color.green;
			}
			else
			{
				Gizmos.color = Color.grey;
			}
			Gizmos.DrawWireCube(objs[i].position, Vector3.one);
		}

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(point, Vector3.one);
		Gizmos.DrawWireSphere(point, ret.dis);


		//Camera camera = Camera.main;
		//Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		//Gizmos.color = Color.red;
		//Gizmos.DrawRay(ray);

		//bvhtree.TraversalTree(delegate(BVHtree.Node node)
		//{
		//	Gizmos.color = Color.grey;
		//	Gizmos.DrawWireCube(node.bounds.center, node.bounds.size);
		//	return true;
		//});

		//bvhtree.TraversalTree(delegate(BVHtree.Node node)
		//{
		//	if (node.bounds.IntersectRay(ray))
		//	{
		//		Gizmos.color = Color.green;
		//		Gizmos.DrawWireCube(node.bounds.center, node.bounds.size);
		//		return true;
		//	}
		//	return false;
		//});
	}

}
