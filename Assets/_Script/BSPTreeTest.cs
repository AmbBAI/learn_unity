using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BSPTreeTest : MonoBehaviour {

	public GameObject meshRoot;

	List<BSPtreeObject> objs;
	BSPtree<BSPtreeObject> bsptree = new BSPtree<BSPtreeObject>();

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

		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		Dictionary<int, int> verticesHash = new Dictionary<int, int>();

		bsptree.TraverseTree(delegate(BSPtreeObject obj)
		{
			for (int i = 0; i < 3; ++i)
			{
				int index = -1;
				int hashCode = obj.vertices[i].GetHashCode();
				if (verticesHash.ContainsKey(hashCode))
				{
					index = verticesHash[hashCode];
				}
				else
				{
					index = vertices.Count;
					vertices.Add(obj.vertices[i]);
					verticesHash[hashCode] = index;
				}
				triangles.Add(index);
			}
		});

		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();

		MeshFilter outMeshFliter = GetComponent<MeshFilter>();
		outMeshFliter.mesh = mesh;
	}

	//void OnDrawGizmos()
	//{
	//	if (!Application.isPlaying) return;
	//	if (!enabled) return;

	//	bsptree.TraverseTree();
	//}
}
