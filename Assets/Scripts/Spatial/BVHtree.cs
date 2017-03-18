using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BVHtree {

	public class Node
	{
		public Bounds bounds;
		public Node left = null;
		public Node right = null;
		public List<Bounds> objs = new List<Bounds>();
	}

	Node root = null;

	public delegate bool TraversalDelegate(Node node);

	int nodeObjCount = 10;
	int maxDepth = -1;

	public void BuildTree(List<Bounds> objList, int nodeObjCount = 10, int maxDepth = -1)
	{
		root = new Node();
		root.objs = objList;
		this.nodeObjCount = nodeObjCount;
		this.maxDepth = maxDepth;
		RecursiveBuildTree(root, 0);
	}

	void RecursiveBuildTree(Node node, int depth)
	{
		if (node.objs.Count <= 0) return;
		node.bounds = node.objs[0];
		foreach (var obj in node.objs) node.bounds.Encapsulate(obj);
		if (node.objs.Count <= nodeObjCount) return;
		if (maxDepth != -1 && depth >= maxDepth) return;

		Vector3 avg = Vector3.zero;
		foreach (var obj in node.objs) avg += obj.center;
		avg /= node.objs.Count;

		Vector3 div = Vector3.zero;
		foreach (var obj in node.objs)
		{
			Vector3 diff = obj.center - avg;
			diff.Scale(diff);
			div += diff;
		}

		Axis axis = Axis.None;
		if (div.x >= Mathf.Max(div.y, div.z)) axis = Axis.X;
		else if (div.y >= div.z) axis = Axis.Y;
		else axis = Axis.Z;

		List<Bounds> leftObjs = new List<Bounds>();
		List<Bounds> rightObjs = new List<Bounds>();
		foreach (var obj in node.objs)
		{
			if (obj.center[(int)axis] < avg[(int)axis])
				leftObjs.Add(obj);
			else rightObjs.Add(obj);
		}

		if (leftObjs.Count <= 0 || rightObjs.Count <= 0) return;
		else
		{
			node.left = new Node();
			node.left.objs = leftObjs;
			node.right = new Node();
			node.right.objs = rightObjs;
			node.objs.Clear();

			RecursiveBuildTree(node.left, depth+1);
			RecursiveBuildTree(node.right, depth+1);
		}
	}

	void ClearTree()
	{
		root = null;
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
			if (node.left != null) RecursiveTraversalTree(node.left, action);
			if (node.right != null) RecursiveTraversalTree(node.right, action);
		}
	}
}
