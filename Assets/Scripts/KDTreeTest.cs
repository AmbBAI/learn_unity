using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KDTreeTest : MonoBehaviour {

	List<KDtreeObject> objs;
	Vector3 point;
	public float moveScale = 10f;

	KDtree<KDtreeObject> kdtree = new KDtree<KDtreeObject>();

	void Start()
	{
		Bounds bounds = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(1000f, 1000f, 0f));
		objs = KDtreeObject.CreateKDTreeObjectPool(bounds, 10000);

		kdtree.BuildTree(objs);
		point = Vector3.zero;
	}

	void Update () {
		if (Input.GetKey(KeyCode.W)) point += Vector3.forward * (moveScale * Time.deltaTime);
		if (Input.GetKey(KeyCode.A)) point += Vector3.left * (moveScale * Time.deltaTime);
		if (Input.GetKey(KeyCode.S)) point += Vector3.back * (moveScale * Time.deltaTime);
		if (Input.GetKey(KeyCode.D)) point += Vector3.right * (moveScale * Time.deltaTime);
		if (Input.GetKey(KeyCode.Q)) point += Vector3.up * (moveScale * Time.deltaTime);
		if (Input.GetKey(KeyCode.E)) point += Vector3.down * (moveScale * Time.deltaTime);
	}

	void OnDrawGizmos()
	{
		if (!Application.isPlaying) return;
		if (!enabled) return;

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
	}
}
