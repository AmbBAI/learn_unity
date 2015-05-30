using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BIHtree {

	public class BIHtreeNode
	{
		public Bounds bounds;
		public BIHtreeNode left = null;
		public BIHtreeNode right = null;
		public List<Bounds> objs = new List<Bounds>();
	}

	BIHtreeNode root = new BIHtreeNode();

	delegate bool NodeSplitJudgeFunc(Bounds bounds);
	public delegate bool TraversalDelegate(BIHtreeNode node);

	int nodeObjCount = 10;
	int maxDepth = -1;

	public void BuildTree(List<Bounds> objList, int nodeObjCount = 10, int maxDepth = -1)
	{
		root.objs = objList;
		this.nodeObjCount = nodeObjCount;
		this.maxDepth = maxDepth;
		RecursiveBuildTree(root, 0);
	}

	void RecursiveBuildTree(BIHtreeNode node, int depth)
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
			node.left = new BIHtreeNode();
			node.left.objs = leftObjs;
			node.right = new BIHtreeNode();
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

	public void DrawTree()
	{
		BoundsRenderer render = BoundsRenderer.Instance;
		if (render == null) return;

		Camera camera = Camera.main;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);

		TraversalTree(root, delegate(BIHtreeNode node)
		{
			render.AddBounds(node.bounds, Color.grey);
			return true;

			//if (node.bounds.IntersectRay(ray))
			//{
			//	render.AddBounds(node.bounds, Color.green);
			//	return true;
			//}
			//return false;
		});
	}

	public void TraversalTree(BIHtreeNode node, TraversalDelegate action)
	{
		if (action(node))
		{
			if (node.left != null) TraversalTree(node.left, action);
			if (node.right != null) TraversalTree(node.right, action);
		}
	}
}
