using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class BSPtreeObject
{
	public Vector3[] vertices = new Vector3[3];
	public virtual Plane plane
	{
		get
		{
			Vector3 normal = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]);
			normal.Normalize();
			return new Plane(normal, Vector3.Dot(vertices[0], normal));
		}
	}

	static public List<BSPtreeObject> CreateBSPtreeObjectWithMesh(List<Mesh> meshes)
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

	public void Split<T>(Plane plane, ref List<T> front, ref List<T> back) where T : BSPtreeObject
	{
		bool s0 = SpatialUtils.SideToPlane(plane, vertices[0]);
		bool s1 = SpatialUtils.SideToPlane(plane, vertices[1]);
		bool s2 = SpatialUtils.SideToPlane(plane, vertices[2]);

		if (s0 && s1 && s2) front.Add(this as T);
		else if (!(s0 || s1 || s2)) back.Add(this as T);
		else if (s0 && !(s1 || s2)) SplitObject(plane, 0, 1, 2, ref front, ref back);
		else if (s1 && !(s2 || s0)) SplitObject(plane, 1, 2, 0, ref front, ref back);
		else if (s2 && !(s0 || s1)) SplitObject(plane, 2, 0, 1, ref front, ref back);
		else if (s0 && s1 && !s2) SplitObject(plane, 2, 0, 1, ref back, ref front);
		else if (s2 && s0 && !s1) SplitObject(plane, 1, 2, 0, ref back, ref front);
		else if (s1 && s2 && !s0) SplitObject(plane, 0, 1, 2, ref back, ref front);
	}

	static Vector3[] gTriCoord = { new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f) };
	void SplitObject<T>(Plane plane, int i0, int i1, int i2, ref List<T> left, ref List<T> right) where T : BSPtreeObject
	{
		float t1 = 0f, t2 = 0f;
		if (!SplitTriangle(plane, vertices[i0], vertices[i1], vertices[i2], ref t1, ref t2))
		{
			Debug.LogWarning(string.Format("!!! t1 {0} ,, t2 {1}", t1, t2));
		}

		t1 = Mathf.Clamp01(t1);
		t2 = Mathf.Clamp01(t2);

		if (Mathf.Approximately(t1, 0f) || Mathf.Approximately(t2, 0f)) { right.Add(this as T); return; }
		if (Mathf.Approximately(t1, 1f) && Mathf.Approximately(t2, 1f)) { left.Add(this as T); return; }

		Vector3 coord1 = gTriCoord[i0] * (1f - t1) + gTriCoord[i1] * t1;
		Vector3 coord2 = gTriCoord[i0] * (1f - t2) + gTriCoord[i2] * t2;

		if (Mathf.Approximately(t1, 1f))
		{
			left.Add(CreateSubObject(gTriCoord[i0], gTriCoord[i1], coord2) as T);
			right.Add(CreateSubObject(gTriCoord[i1], gTriCoord[i2], coord2) as T);
			return;
		}
		if (Mathf.Approximately(t2, 1f))
		{
			left.Add(CreateSubObject(gTriCoord[i0], coord1, gTriCoord[i2]) as T);
			right.Add(CreateSubObject(gTriCoord[i1], gTriCoord[i2], coord1) as T);
			return;
		}

		left.Add(CreateSubObject(gTriCoord[i0], coord1, coord2) as T);
		right.Add(CreateSubObject(gTriCoord[i1], coord2, coord1) as T);
		right.Add(CreateSubObject(gTriCoord[i1], gTriCoord[i2], coord2) as T);

	}

	static bool SplitTriangle(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2, ref float t1, ref float t2)
	{
		t1 = SpatialUtils.SplitSegment(plane, v0, v1);
		t2 = SpatialUtils.SplitSegment(plane, v0, v2);
		if (t1 < 0f || t1 > 1f) return false;
		if (t2 < 0f || t2 > 1f) return false;
		return true;
	}

	public virtual BSPtreeObject CreateSubObject(Vector3 a, Vector3 b, Vector3 c)
	{
		BSPtreeObject subObj = new BSPtreeObject();
		subObj.vertices[0] = vertices[0] * a.x + vertices[1] * a.y + vertices[2] * a.z;
		subObj.vertices[1] = vertices[0] * b.x + vertices[1] * b.y + vertices[2] * b.z;
		subObj.vertices[2] = vertices[0] * c.x + vertices[1] * c.y + vertices[2] * c.z;
		return subObj;
	}

	public enum UbietyResult
	{
		Cross = 2,
		Front = 1,
		Coincident = 0,
		Back = -1,
	}
	public UbietyResult UbietyToPlane(Plane plane)
	{
		if (SpatialUtils.PlaneCoincide(plane, this.plane)) return UbietyResult.Coincident;

		bool s0 = SpatialUtils.SideToPlane(plane, vertices[0]);
		bool s1 = SpatialUtils.SideToPlane(plane, vertices[1]);
		bool s2 = SpatialUtils.SideToPlane(plane, vertices[2]);

		if (s0 && s1 && s2) return UbietyResult.Front;
		else if (!(s0 || s1 || s2)) return UbietyResult.Back;
		else return UbietyResult.Cross;
	}

	public virtual void Write(BinaryWriter writer)
	{
		for (int i=0; i<3; ++i)
		{
			writer.Write(vertices[i].x);
			writer.Write(vertices[i].y);
			writer.Write(vertices[i].z);
		}
	}

	public virtual void Read(BinaryReader reader)
	{
		for (int i = 0; i < 3; ++i)
		{
			vertices[i].x = reader.ReadSingle();
			vertices[i].y = reader.ReadSingle();
			vertices[i].z = reader.ReadSingle();
		}
	}
}

public class BSPtree<T> where T : BSPtreeObject, new()
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
			if (SpatialUtils.PlaneCoincide(splitPlane, obj.plane))
				nodeObjs.Add(obj);
			else
				obj.Split(splitPlane, ref frontObjs, ref backObjs);
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
			Plane plane = selected.plane;
			int frontCount = 0;
			int backCount = 0;
			int splitCount = 0;

			foreach (var obj in objs)
			{
				switch (obj.UbietyToPlane(plane))
				{
					case BSPtreeObject.UbietyResult.Cross:
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
		bool side = SpatialUtils.SideToPlane(node.plane, point);
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

	public bool IntersectPoint(Vector3 point)
	{
		if (root == null) return false;
		return RecursiveIntersectPoint(root, point);
	}

	bool RecursiveIntersectPoint(Node node, Vector3 point)
	{
		bool side = SpatialUtils.SideToPlane(node.plane, point);

		if (side)
		{
			if (node.left != null) return RecursiveIntersectPoint(node.left, point);
			return false;
		}
		else
		{
			if (node.right != null) return RecursiveIntersectPoint(node.right, point);
			return true;
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
				T obj = new T();
				obj.Read(reader);
				node.objs.Add(obj);
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
