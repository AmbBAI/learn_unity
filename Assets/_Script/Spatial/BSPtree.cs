using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BSPtreeObject
{
	public Mesh mesh = null;
	public int triangle;

}

public class BSPtree {

	class Node
	{
		public Node left = null;
		public Node right = null;
		public List<BSPtreeObject> objs = null;
	}

	Node root = null;

	public void BuildTree(List<Mesh> meshes)
	{
		List<BSPtreeObject> objs = new List<BSPtreeObject>();
		foreach (var mesh in meshes)
		{
			int triangleCount = mesh.triangles.Length / 3;
			for (int i=0; i<triangleCount; ++i)
			{
				var bspObj = new BSPtreeObject();
				bspObj.mesh = mesh;
				bspObj.triangle = i;
				objs.Add(bspObj);
			}
		}

		root = new Node();
		root.objs = objs;
		RecursiveBuildTree(root);
	}

	void RecursiveBuildTree(Node node)
	{
		var splitObj = SelectSplitObject(node.objs);
		Plane splitPlane = CalcSplitPlane(splitObj);

		List<BSPtreeObject> leftObjs = new List<BSPtreeObject>();
		List<BSPtreeObject> rightObjs = new List<BSPtreeObject>();
		foreach (var obj in node.objs)
		{
			if (splitObj != obj)
			{
				int splitRet = CheckSide(splitPlane, obj);
				if (splitRet >= 0) leftObjs.Add(obj);
				if (splitRet <= 0) rightObjs.Add(obj);
			}
		}

		node.objs.Clear();
		node.objs.Add(splitObj);

		if (leftObjs.Count > 0)
		{
			node.left = new Node();
			node.objs = leftObjs;
			RecursiveBuildTree(node.left);
		}
		
		if (rightObjs.Count > 0)
		{
			node.right = new Node();
			node.objs = rightObjs;
			RecursiveBuildTree(node.right);
		}
	}

	BSPtreeObject SelectSplitObject(List<BSPtreeObject> objs)
	{
		return objs[UnityEngine.Random.Range(0, objs.Count)];
	}

	Plane CalcSplitPlane(BSPtreeObject obj)
	{
		int i0 = obj.mesh.triangles[obj.triangle * 3];
		int i1 = obj.mesh.triangles[obj.triangle * 3 + 1];
		int i2 = obj.mesh.triangles[obj.triangle * 3 + 2];

		Vector3 v0 = obj.mesh.vertices[i0];
		Vector3 v1 = obj.mesh.vertices[i1];
		Vector3 v2 = obj.mesh.vertices[i2];

		return new Plane(v0, v1, v2);
	}

	int CheckSide(Plane plane, BSPtreeObject obj)
	{
		int i0 = obj.mesh.triangles[obj.triangle * 3];
		int i1 = obj.mesh.triangles[obj.triangle * 3 + 1];
		int i2 = obj.mesh.triangles[obj.triangle * 3 + 2];

		Vector3 v0 = obj.mesh.vertices[i0];
		Vector3 v1 = obj.mesh.vertices[i1];
		Vector3 v2 = obj.mesh.vertices[i2];

		bool s0 = plane.GetSide(v0);
		bool s1 = plane.GetSide(v1);
		bool s2 = plane.GetSide(v2);

		if (s0 && s1 && s2) return 1;
		if (!(s0 || s1 || s2)) return -1;
		return 0;
	}
}
