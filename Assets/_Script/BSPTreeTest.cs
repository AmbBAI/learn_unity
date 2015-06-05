using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BSPTreeTest : MonoBehaviour {

	public GameObject meshRoot;

	List<BSPtreeObject> objs;
	BSPtree<BSPtreeObject> bsptree = new BSPtree<BSPtreeObject>();

	Dictionary<Vector3, int> verticesHash = new Dictionary<Vector3, int>();
	int[] triangles = null;
	Mesh mesh = null;

	bool isInit = false;

	void Start()
	{
		var meshList = meshRoot.GetComponentsInChildren<MeshFilter>();
		if (meshList.Length <= 0) return;
		meshRoot.SetActive(false);

		List<Mesh> meshes = new List<Mesh>();
		foreach (var meshFliter in meshList)
		{
			meshes.Add(meshFliter.mesh);
		}

		objs = BSPtreeObject.InitWithMesh(meshes);
		bsptree.BuildTree(objs);

		mesh = new Mesh();
		MeshFilter outMeshFliter = GetComponent<MeshFilter>();
		outMeshFliter.mesh = mesh;
	}

	void Update()
	{
		Matrix4x4 mat = transform.worldToLocalMatrix;
		Vector3 pos = mat.MultiplyPoint3x4(Camera.main.transform.position);

		if (!isInit)
		{
			List<Vector3> vtxList = new List<Vector3>();
			List<int> triList = new List<int>();
			bsptree.TraverseTree(pos, delegate(BSPtreeObject obj)
			{
				for (int i = 0; i < 3; ++i)
				{
					int index = -1;
					if (verticesHash.ContainsKey(obj.vertices[i]))
					{
						index = verticesHash[obj.vertices[i]];
					}
					else
					{
						index = vtxList.Count;
						vtxList.Add(obj.vertices[i]);
						verticesHash[obj.vertices[i]] = index;
					}
					triList.Add(index);
				}
			});

			mesh.vertices = vtxList.ToArray();
			mesh.triangles = triangles = triList.ToArray();

			isInit = true;
		}
		else
		{
			int triIndex = 0;
			bsptree.TraverseTree(pos, delegate(BSPtreeObject obj)
			{
				for (int i = 0; i < 3; ++i)
				{
					int index = verticesHash[obj.vertices[i]];
					triangles[triIndex] = index;
					++ triIndex;
				}
			});

			mesh.triangles = triangles;
		}
	}

	//void OnDrawGizmos()
	//{
	//	if (!Application.isPlaying) return;
	//	if (!enabled) return;

	//	bsptree.TraverseTree();
	//}
}
