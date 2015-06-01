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
	class Node
	{
		public int objIndex;
		public int objCount;

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
	List<T> objs = null;

	public class XAxisComparer : IComparer<T>
	{
		public int Compare(T a, T b) { return a.position.x.CompareTo(b.position.x); }
	}

	public class YAxisComparer : IComparer<T>
	{
		public int Compare(T a, T b) { return a.position.y.CompareTo(b.position.y); }
	}

	public class ZAxisComparer : IComparer<T>
	{
		public int Compare(T a, T b) { return a.position.z.CompareTo(b.position.z); }
	}

	IComparer<T>[] comparer = { new XAxisComparer(), new YAxisComparer(), new ZAxisComparer() };

	public void BuildTree(List<T> objList)
	{
		ClearTree();
		if (objList.Count <= 0) return;

		root = new Node();
		root.objIndex = 0;
		root.objCount = objList.Count;
		//root.objs = new List<T>(objList);
		objs = new List<T>(objList);
		RecursiveBuildTree(root);
	}

	void RecursiveBuildTree(Node node)
	{
		if (node.objCount <= 1) return;

		Vector3 avg = Vector3.zero;
		for (int i = 0; i < node.objCount; ++i) avg += objs[node.objIndex + i].position;
		avg /= node.objCount;

		Vector3 div = Vector3.zero;
		for (int i = 0; i < node.objCount; ++i)
		{
			Vector3 diff = objs[node.objIndex + i].position - avg;
			diff.Scale(diff);
			div += diff;
		}

		Axis axis = Axis.None;
		if (div.x >= Mathf.Max(div.y, div.z)) axis = Axis.X;
		else if (div.y >= div.z) axis = Axis.Y;
		else axis = Axis.Z;

		objs.Sort(node.objIndex, node.objCount, comparer[(int)axis]);

		float splitValue = objs[node.objIndex + node.objCount / 2].position[(int)axis];
		int leftIndex = node.objIndex, leftCount = 0;
		int rightIndex = node.objIndex + node.objCount, rightCount = 0;
		for (int i = 0; i < node.objCount; ++i)
		{
			if (objs[i + node.objIndex].position[(int)axis] < splitValue)
			{
				leftCount = i + 1;
			}
			else if (objs[i + node.objIndex].position[(int)axis] > splitValue)
			{
				rightIndex = i + node.objIndex;
				rightCount = node.objCount - i;
				break;
			}
		}

		node.objIndex = leftIndex + leftCount;
		node.objCount = node.objCount - leftCount - rightCount;
		if (leftCount <= 0 && rightCount <= 0) return;
		else
		{
			node.splitAxis = axis;
			node.splitValue = splitValue;
			if (leftCount > 0)
			{
				node.left = new Node();
				node.left.objIndex = leftIndex;
				node.left.objCount = leftCount;
				RecursiveBuildTree(node.left);
			}
			
			if (rightCount > 0)
			{
				node.right = new Node();
				node.right.objIndex = rightIndex;
				node.right.objCount = rightCount;
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
		for (int i = 0; i < node.objCount; ++i)
		{
			float dis = Vector3.Distance(objs[node.objIndex + i].position, point);
			if (ret.obj == null || dis < ret.dis)
			{
				ret.obj = objs[node.objIndex + i];
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
