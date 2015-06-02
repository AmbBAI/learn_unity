using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Octree {

	static Vector3[] gBoundsSplit = {new Vector3(1f, 1f, 1f),
								 new Vector3(1f, 3f, 1f),
								 new Vector3(1f, 1f, 3f),
								 new Vector3(1f, 3f, 3f),
								 new Vector3(3f, 1f, 1f),
								 new Vector3(3f, 3f, 1f),
								 new Vector3(3f, 1f, 3f),
								 new Vector3(3f, 3f, 3f)};

	public class Node
	{
		public Bounds bounds;
		public List<Node> children = new List<Node>();
		public List<Bounds> objs = new List<Bounds>();
	}

	Node root = null;

	public delegate bool TraversalDelegate(Node node);

	public void BuildTree(List<Bounds> objList, Bounds bounds)
	{
		root = new Node();
		root.bounds = bounds;
		root.objs = objList;
		RecursiveBuildTree(root);
	}

	void RecursiveBuildTree(Node node)
	{
		List<Bounds> nodeObjs = new List<Bounds>();
		List<Bounds>[] childObjs = new List<Bounds>[8];
		Bounds[] childBounds = new Bounds[8];
		for (int i=0; i<8; ++i)
		{
			childBounds[i].center = Vector3.Scale(gBoundsSplit[i], node.bounds.size / 4f) + node.bounds.min;
			childBounds[i].size = node.bounds.size / 2f;
			childObjs[i] = new List<Bounds>();
		}

		foreach (var obj in node.objs)
		{
			int idx = -1;
			for (int i=0; i<8; ++i)
			{
				if (Vector3.Max(obj.min, childBounds[i].min).Equals(obj.min)
					&& Vector3.Min(obj.max, childBounds[i].max).Equals(obj.max))
				{
					if (idx == -1) idx = i;
					else idx = -1;
				}
			}
			if (idx == -1) nodeObjs.Add(obj);
			else childObjs[idx].Add(obj);
		}

		node.objs = nodeObjs;
		for (int i=0; i<8; ++i)
		{
			if (childObjs[i].Count <= 0) continue;
			Node cNode = new Node();
			cNode.bounds = childBounds[i];
			cNode.objs = childObjs[i];
			node.children.Add(cNode);
			RecursiveBuildTree(cNode);
		}
	}

	public void TraversalTree(TraversalDelegate action)
	{
		if (root == null) return;
		RecursiveTraversalTree(root, action);
	}

	void RecursiveTraversalTree(Node node, TraversalDelegate action)
	{
		if (action(node))
		{
			foreach (var child in node.children)
			{
				RecursiveTraversalTree(child, action);
			}
		}
	}
}
