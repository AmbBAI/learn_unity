using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BSPtreeObject
{
	public Vector3[] vertices = new Vector3[3];

	static public List<BSPtreeObject> InitWithMesh(List<Mesh> meshes)
	{
		List<BSPtreeObject> objs = new List<BSPtreeObject>();
		foreach (var mesh in meshes)
		{
			int triangleCount = mesh.triangles.Length / 3;
			for (int i = 0; i < triangleCount; ++i)
			{
				var bspObj = new BSPtreeObject();
				int i0 = mesh.triangles[i * 3];
				int i1 = mesh.triangles[i * 3 + 1];
				int i2 = mesh.triangles[i * 3 + 2];
				bspObj.vertices[0] = mesh.vertices[i0];
				bspObj.vertices[1] = mesh.vertices[i1];
				bspObj.vertices[2] = mesh.vertices[i2];
				objs.Add(bspObj);
			}
		}
		return objs;
	}

	static T CreateSubObject<T>(T obj, Vector3 a, Vector3 b, Vector3 c) where T : BSPtreeObject, new()
	{
		T subObj = new T();
		subObj.vertices[0] = obj.vertices[0] * a.x + obj.vertices[1] * a.y + obj.vertices[2] * a.z;
		subObj.vertices[1] = obj.vertices[0] * b.x + obj.vertices[1] * b.y + obj.vertices[2] * b.z;
		subObj.vertices[2] = obj.vertices[0] * c.x + obj.vertices[1] * c.y + obj.vertices[2] * c.z;
		return subObj;
	}

	static public void Split<T>(Plane plane, T obj, ref List<T> front, ref List<T> back) where T : BSPtreeObject, new()
	{
		bool s0 = plane.GetSide(obj.vertices[0]);
		bool s1 = plane.GetSide(obj.vertices[1]);
		bool s2 = plane.GetSide(obj.vertices[2]);

		if (s0 && s1 && s2) front.Add(obj);
		else if (!(s0 || s1 || s2)) back.Add(obj);
		else if (s0 && !(s1 || s2)) SplitObject(plane, obj, 0, 1, 2, ref front, ref back);
		else if (s1 && !(s2 || s0)) SplitObject(plane, obj, 1, 2, 0, ref front, ref back);
		else if (s2 && !(s0 || s1)) SplitObject(plane, obj, 2, 0, 1, ref front, ref back);
		else if (s0 && s1 && !s2) SplitObject(plane, obj, 2, 0, 1, ref back, ref front);
		else if (s2 && s0 && !s1) SplitObject(plane, obj, 1, 2, 0, ref back, ref front);
		else if (s1 && s2 && !s0) SplitObject(plane, obj, 0, 1, 2, ref back, ref front);
	}

	static Vector3[] gTriCoord = {new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f)};
	static void SplitObject<T>(Plane plane, T obj, int i0, int i1, int i2, ref List<T> left, ref List<T> right) where T : BSPtreeObject, new()
	{
		float t1 = 0f, t2 = 0f;
		SplitTriangle(plane, obj.vertices[i0], obj.vertices[i1], obj.vertices[i2], ref t1, ref t2);
		Vector3 coord1 = gTriCoord[i0] * (1f - t1) + gTriCoord[i1] * t1;
		Vector3 coord2 = gTriCoord[i0] * (1f - t2) + gTriCoord[i2] * t2;

		left.Add(CreateSubObject(obj, gTriCoord[i0], coord1, coord2));
		right.Add(CreateSubObject(obj, gTriCoord[i1], coord2, coord1));
		right.Add(CreateSubObject(obj, gTriCoord[i1], gTriCoord[i2], coord2));
	}

	static bool SplitTriangle(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2, ref float t1, ref float t2)
	{
		t1 = SplitSegment(plane, v0, v1);
		if (t1 < 0f || t1 > 1f) return false;
		t2 = SplitSegment(plane, v0, v2);
		if (t2 < 0f || t2 > 1f) return false;
		return true;
	}

	static float SplitSegment(Plane plane, Vector3 v0, Vector3 v1)
	{
		return (plane.distance - Vector3.Dot(plane.normal, v0)) / Vector3.Dot(plane.normal, v1 - v0);
	}
}

public class BSPtree <T> where T : BSPtreeObject, new()
{

	class Node
	{
		public Node left = null;
		public Node right = null;
		public List<T> objs = null;
	}

	Node root = null;

	public void BuildTree(List<T> objs)
	{
		root = new Node();
		root.objs = new List<T>(objs);
		RecursiveBuildTree(root);
	}

	void RecursiveBuildTree(Node node)
	{
		if (node.objs.Count <= 1) return;

		var splitObj = SelectSplitObject(node.objs);
		Plane splitPlane = CalcSplitPlane(splitObj);

		List<T> mid = new List<T>() { splitObj };
		List<T> front = new List<T>();
		List<T> back = new List<T>();
		foreach (var obj in node.objs)
		{
			if (splitObj != obj)
			{
				Plane plane = CalcSplitPlane(obj);
				if (plane.normal == splitPlane.normal && Mathf.Approximately(plane.distance, splitPlane.distance))
					mid.Add(obj);
				else
					BSPtreeObject.Split(splitPlane, obj, ref front, ref back);
			}
		}

		node.objs.Clear();
		node.objs = mid;

		if (front.Count > 0)
		{
			node.left = new Node();
			node.left.objs = front;
			RecursiveBuildTree(node.left);
		}

		if (back.Count > 0)
		{
			node.right = new Node();
			node.right.objs = back;
			RecursiveBuildTree(node.right);
		}
	}

	T SelectSplitObject(List<T> objs)
	{
		//int idx = UnityEngine.Random.Range(0, objs.Count);
		return objs[0];
	}

	Plane CalcSplitPlane(T obj)
	{
		return new Plane(obj.vertices[0], obj.vertices[1], obj.vertices[2]);
	}

	public void TraverseTree()
	{
		if (root == null) return;
		UnityEngine.Random.seed = 0;
		RecursiveTraverseTree(root);
	}

	void RecursiveTraverseTree(Node node)
	{
		if (node.left != null)
		{
			RecursiveTraverseTree(node.left);
		}

		Gizmos.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
		foreach (var obj in node.objs)
		{
			Gizmos.DrawLine(obj.vertices[0], obj.vertices[1]);
			Gizmos.DrawLine(obj.vertices[1], obj.vertices[2]);
			Gizmos.DrawLine(obj.vertices[2], obj.vertices[0]);
		}

		if (node.right != null)
		{
			RecursiveTraverseTree(node.right);
		}
	}
}
