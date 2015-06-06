using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class BSPtreeObject
{
	public Vector3[] vertices = new Vector3[3];

	public virtual Plane GetPlane()
	{
		return new Plane(vertices[0], vertices[1], vertices[2]);
	}

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
		bool s0 = SideToPlane(plane, obj.vertices[0]);
		bool s1 = SideToPlane(plane, obj.vertices[1]);
		bool s2 = SideToPlane(plane, obj.vertices[2]);

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
		if (SplitTriangle (plane, obj.vertices [i0], obj.vertices [i1], obj.vertices [i2], ref t1, ref t2))
		{
			if (Mathf.Approximately(t1, 0f) || Mathf.Approximately(t2, 0f)) { right.Add(obj); return; }
			if (Mathf.Approximately(t1, 1f) && Mathf.Approximately(t2, 1f)) { left.Add(obj); return; }

			Vector3 coord1 = gTriCoord[i0] * (1f - t1) + gTriCoord[i1] * t1;
			Vector3 coord2 = gTriCoord[i0] * (1f - t2) + gTriCoord[i2] * t2;

			left.Add(CreateSubObject(obj, gTriCoord[i0], coord1, coord2));
			right.Add(CreateSubObject(obj, gTriCoord[i1], coord2, coord1));
			right.Add(CreateSubObject(obj, gTriCoord[i1], gTriCoord[i2], coord2));
		}
		else
		{
			Debug.LogWarning(string.Format("!!! t1 {0} ,, t2 {1}", t1, t2));
		}
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

	public enum UbietyResult
	{
		Cross = 2,
		Front = 1,
		Coincident = 0,
		Back = -1,
	}
	static public UbietyResult UbietyToPlane<T>(Plane plane, T obj) where T : BSPtreeObject
	{
		if (PlaneCoincide(plane, obj.GetPlane())) return UbietyResult.Coincident;

		bool s0 = SideToPlane(plane, obj.vertices[0]);
		bool s1 = SideToPlane(plane, obj.vertices[1]);
		bool s2 = SideToPlane(plane, obj.vertices[2]);

		if (s0 && s1 && s2) return UbietyResult.Front;
		else if (!(s0 || s1 || s2)) return UbietyResult.Back;
		else return UbietyResult.Cross;
	}

	static public bool SideToPlane(Plane plane, Vector3 position)
	{
		return Vector3.Dot(plane.normal, position) >= plane.distance;
	}

	static public bool PlaneCoincide(Plane a, Plane b)
	{
		return a.normal == b.normal && Mathf.Approximately(a.distance, b.distance);
	}

	public virtual void Write(BinaryWriter writer)
	{
		for (int i=0; i<3; ++i)
		{
			writer.Write(vertices[i].x);
			writer.Write(vertices[i].y);
			writer.Write(vertices[i].z);
		}
		writer.Flush();
	}

	static public T Read<T>(BinaryReader reader) where T : BSPtreeObject, new()
	{
		T obj = new T();
		for (int i = 0; i < 3; ++i)
		{
			obj.vertices[i].x = reader.ReadSingle();
			obj.vertices[i].y = reader.ReadSingle();
			obj.vertices[i].z = reader.ReadSingle();
		}
		return obj;
	}
}

public class BSPtree <T> where T : BSPtreeObject, new()
{

	class Node
	{
		public Plane plane;
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

		Plane splitPlane = PickSplittingPlane(node.objs);

		List<T> nodeObjs = new List<T>();
		List<T> frontObjs = new List<T>();
		List<T> backObjs = new List<T>();
		foreach (var obj in node.objs)
		{
			if (BSPtreeObject.PlaneCoincide(splitPlane, obj.GetPlane()))
				nodeObjs.Add(obj);
			else
				BSPtreeObject.Split(splitPlane, obj, ref frontObjs, ref backObjs);
		}

		node.plane = splitPlane;
		node.objs = nodeObjs;

		if (frontObjs.Count > 0)
		{
			node.left = new Node();
			node.left.objs = frontObjs;
			RecursiveBuildTree(node.left);
		}

		if (backObjs.Count > 0)
		{
			node.right = new Node();
			node.right.objs = backObjs;
			RecursiveBuildTree(node.right);
		}
	}

	Plane PickSplittingPlane(List<T> objs)
	{
		const float k = 0.8f;
		Plane retPlane = new Plane();
		float bestScore = Mathf.Infinity;

		foreach (var selected in objs)
		{
			Plane plane = selected.GetPlane();
			int frontCount = 0;
			int backCount = 0;
			int splitCount = 0;

			foreach (var obj in objs)
			{
				if (obj != selected)
				{
					switch(BSPtreeObject.UbietyToPlane(plane, obj))
					{
						case BSPtreeObject.UbietyResult.Coincident:
							splitCount += 1;
							break;
						case BSPtreeObject.UbietyResult.Front:
							frontCount += 1;
							break;
						case BSPtreeObject.UbietyResult.Back:
							backCount += 1;
							break;
					}
				}
			}

			float score = k * splitCount + (1f - k) * Mathf.Abs(frontCount - backCount);
			if (score < bestScore)
			{
				retPlane = plane;
				bestScore = score;
			}
		}

		return retPlane;
	}

	public void TraverseTree(Vector3 point, Action<T> action)
	{
		if (root == null) return;
		RecursiveTraverseTree(root, point, action);
	}

	void RecursiveTraverseTree(Node node, Vector3 point, Action<T> action)
	{
		bool side = BSPtreeObject.SideToPlane(node.plane, point);
		Node a = node.right, b = node.left;
		if (!side) { a = node.left; b = node.right; }

		if (a != null)
		{
			RecursiveTraverseTree(a, point, action);
		}

		if (action != null)
		{
			foreach (var obj in node.objs)
			{
				action(obj);
			}
		}

		if (b != null)
		{
			RecursiveTraverseTree(b, point, action);
		}
	}

	public void SaveToFile(string name)
	{
		if (root == null) return;

		string path = string.Format("Assets/Resources/{0}.bsp", name);
		FileStream fsw = File.Create(path);

		using(BinaryWriter writer = new BinaryWriter(fsw))
		{
			Write(writer);
			writer.Close();
		}
		
		fsw.Close();
	}

	public void LoadFromFile(string name)
	{
		string path = string.Format("Assets/Resources/{0}.bsp", name);
		FileStream fsw = File.OpenRead(path);

		using(BinaryReader reader = new BinaryReader(fsw))
		{
			Read(reader);
			reader.Close();
		}

		fsw.Close();
	}

	public void Write(BinaryWriter writer)
	{
		RecursiveWriteBytes(root, writer);
	}

	public void Read(BinaryReader reader)
	{
		root = RecursiveReadBytes(reader);
	}

	Node RecursiveReadBytes(BinaryReader reader)
	{
		if (reader.ReadBoolean())
		{
			Node node = new Node();
			Vector3 normal = new Vector3();
			normal.x = reader.ReadSingle();
			normal.y = reader.ReadSingle();
			normal.z = reader.ReadSingle();
			float distance = reader.ReadSingle();

			node.plane = new Plane(normal, distance);
			node.left = RecursiveReadBytes(reader);
			node.right = RecursiveReadBytes(reader);

			node.objs = new List<T>();
			int objCount = reader.ReadInt32();
			for (int i=0; i<objCount; ++i)
			{
				node.objs.Add(BSPtreeObject.Read<T>(reader));
			}
			return node;
		}
		else return null;
	}

	void RecursiveWriteBytes(Node node, BinaryWriter writer)
	{
		writer.Write(true);
		Vector3 n = node.plane.normal;
		float d = node.plane.distance;
		writer.Write(n.x);
		writer.Write(n.y);
		writer.Write(n.z);
		writer.Write(d);

		if (node.left != null) RecursiveWriteBytes(node.left, writer);
		else writer.Write(false);
		if (node.right != null) RecursiveWriteBytes(node.right, writer);
		else writer.Write(false);

		writer.Write(node.objs.Count);
		foreach (var obj in node.objs)
		{
			obj.Write(writer);
		}
	}

}
