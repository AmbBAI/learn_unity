using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BVHtree {

	public class BVHtreeNode
	{
		public Bounds bounds;
		public BVHtreeNode left = null;
		public BVHtreeNode right = null;
		public List<Bounds> objs = new List<Bounds>();
	}

	BVHtreeNode root = null;

	delegate bool NodeSplitJudgeFunc(Bounds bounds);
	public delegate bool TraversalDelegate(BVHtreeNode node);

	int nodeObjCount = 10;
	int maxDepth = -1;

	public void BuildTree(List<Bounds> objList, int nodeObjCount = 10, int maxDepth = -1)
	{
		root = new BVHtreeNode();
		root.objs = objList;
		this.nodeObjCount = nodeObjCount;
		this.maxDepth = maxDepth;
		RecursiveBuildTree(root, 0);
	}

	void RecursiveBuildTree(BVHtreeNode node, int depth)
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

		List<Bounds> leftObjs = new List<Bounds>();
		List<Bounds> rightObjs = new List<Bounds>();

		if (div.x >= Mathf.Max(div.y, div.z))
			SplitNodes(node.objs, ref leftObjs, ref rightObjs, _obj => _obj.center.x < avg.x);
		else if (div.y >= div.z)
			SplitNodes(node.objs, ref leftObjs, ref rightObjs, _obj => _obj.center.y < avg.y);
		else SplitNodes(node.objs, ref leftObjs, ref rightObjs, _obj => _obj.center.z < avg.z);

		if (leftObjs.Count <= 0 || rightObjs.Count <= 0) return;
		else
		{
			node.left = new BVHtreeNode();
			node.left.objs = leftObjs;
			node.right = new BVHtreeNode();
			node.right.objs = rightObjs;
			node.objs.Clear();

			RecursiveBuildTree(node.left, depth+1);
			RecursiveBuildTree(node.right, depth+1);
		}
	}


	void SplitNodes(List<Bounds> objs, ref List<Bounds> leftObjs, ref List<Bounds> rightObjs, NodeSplitJudgeFunc judge)
	{
		foreach (var obj in objs)
		{
			if (judge(obj)) leftObjs.Add(obj);
			else rightObjs.Add(obj);
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

	void RecursiveTraversalTree(BVHtreeNode node, TraversalDelegate action)
	{
		if (action(node))
		{
			if (node.left != null) RecursiveTraversalTree(node.left, action);
			if (node.right != null) RecursiveTraversalTree(node.right, action);
		}
	}
}
