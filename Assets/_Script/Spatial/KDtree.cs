using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class KDtreeObject
{
	public Vector3 position;
}

public class KDtree<T> where T : KDtreeObject
{
	public class Node
	{
		public List<T> objs = null;
		public Axis splitAxis = Axis.None;
		public float splitValue;
		public Node left = null;
		public Node right = null;
	}

	public class SearchResult
	{
		public float dis;
		public KDtreeObject obj;
	}

	Node root = null;

	public delegate bool TraversalDelegate(Node node);

	public void BuildTree(List<T> objList)
	{
		ClearTree();
		if (objList.Count <= 0) return;

		root = new Node();
		root.objs = objList;
		RecursiveBuildTree(root);
	}

	void RecursiveBuildTree(Node node)
	{
		if (node.objs.Count <= 0) return;
		if (node.objs.Count <= 1) return;

		Vector3 avg = Vector3.zero;
		foreach (var obj in node.objs) avg += obj.position;
		avg /= node.objs.Count;

		Vector3 div = Vector3.zero;
		foreach (var obj in node.objs)
		{
			Vector3 diff = obj.position - avg;
			diff.Scale(diff);
			div += diff;
		}

		Axis axis = Axis.None;
		if (div.x >= Mathf.Max(div.y, div.z)) axis = Axis.X;
		else if (div.y >= div.z) axis = Axis.Y;
		else axis = Axis.Z;

		node.objs.Sort((a, b) => a.position[(int)axis].CompareTo(b.position[(int)axis]));

		float splitValue = node.objs[node.objs.Count / 2].position[(int)axis];
		List<T> midObjs = new List<T>();
		List<T> leftObjs = new List<T>();
		List<T> rightObjs = new List<T>();
		foreach (var obj in node.objs)
		{
			if (obj.position[(int)axis] == splitValue)
				midObjs.Add(obj);
			if (obj.position[(int)axis] < splitValue)
				leftObjs.Add(obj);
			else rightObjs.Add(obj);
		}

		node.objs = midObjs;
		if (leftObjs.Count <= 0 && rightObjs.Count <= 0) return;
		else
		{
			node.splitAxis = axis;
			node.splitValue = splitValue;
			if (leftObjs.Count > 0)
			{
				node.left = new Node();
				node.left.objs = leftObjs;
				RecursiveBuildTree(node.left);
			}
			
			if (rightObjs.Count > 0)
			{
				node.right = new Node();
				node.right.objs = rightObjs;
				RecursiveBuildTree(node.right);
			}
		}
	}

	void ClearTree()
	{
		root = null;
	}

	public SearchResult SearchNode(Vector3 point)
	{
		SearchResult ret = new SearchResult();
		ret.obj = null;
		ret.dis = -1f;
		if (root == null) return ret;

		RecursiveSearchNode(root, point, ref ret);

		return ret;
	}

	void SelectNode(Node node, Vector3 point, ref SearchResult ret)
	{
		foreach (var obj in node.objs)
		{
			float dis = Vector3.Distance(obj.position, point);
			if (ret.obj == null || dis < ret.dis)
			{
				ret.obj = obj;
				ret.dis = dis;
			}
		}
	}

	void RecursiveSearchNode(Node node, Vector3 point, ref SearchResult ret)
	{
		if (node.splitAxis == Axis.None)
		{
			SelectNode(node, point, ref ret);
		}
		else
		{
			if (point[(int)node.splitAxis] < node.splitValue)
			{
				if (node.left != null)
					RecursiveSearchNode(node.left, point, ref ret);

				if (ret.obj == null || point[(int)node.splitAxis] + ret.dis > node.splitValue)
					SelectNode(node, point, ref ret);

				if (node.right != null
					&& point[(int)node.splitAxis] + ret.dis > node.splitValue)
				{
					RecursiveSearchNode(node.right, point, ref ret);
				}
			}
			else
			{
				if (node.right != null)
					RecursiveSearchNode(node.right, point, ref ret);

				if (ret.obj == null || point[(int)node.splitAxis] - ret.dis < node.splitValue)
					SelectNode(node, point, ref ret);

				if (node.left != null
					&& point[(int)node.splitAxis] - ret.dis < node.splitValue)
				{
					RecursiveSearchNode(node.left, point, ref ret);
				}
			}
		}
	}
}
