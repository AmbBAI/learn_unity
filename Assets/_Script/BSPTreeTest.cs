using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BSPTreeTest : MonoBehaviour {

	public GameObject meshRoot;

	List<BSPtreeObject> objs;
	BSPtree<BSPtreeObject> bsptree = new BSPtree<BSPtreeObject>();

	void Start()
	{
		var meshList = meshRoot.GetComponentsInChildren<MeshFilter>();
		if (meshList.Length <= 0) return;

		List<Mesh> meshes = new List<Mesh>();
		foreach (var meshF in meshList)
		{
			meshes.Add(meshF.mesh);
		}

		objs = BSPtreeObject.InitWithMesh(meshes);
		Debug.Log(objs);
		bsptree.BuildTree(objs);
	}

	void OnDrawGizmos()
	{
		if (!Application.isPlaying) return;
		if (!enabled) return;

		bsptree.TraverseTree();
	}
}
