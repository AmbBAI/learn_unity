using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Axis
{
	None = -1,
	X = 0,
	Y = 1,
	Z = 2,
}

public class SpatialUtils {

	static public List<Bounds> GetMeshTriangleBoundsList(Mesh mesh)
	{
		List<Bounds> ret = new List<Bounds>();

		MeshTopology meshTopo = mesh.GetTopology(0);
		int stride = 3;
		if (meshTopo == MeshTopology.Triangles) stride = 3;
		else if (meshTopo == MeshTopology.Quads) stride = 4;
		else return ret;

		for (int subMesh = 0; subMesh < mesh.subMeshCount; ++subMesh)
		{
			var indices = mesh.GetIndices(subMesh);
			var vertices = mesh.vertices;
			for (int i = 0; i + stride <= indices.Length; i += stride)
			{
				Bounds bounds = new Bounds(vertices[indices[i]], Vector3.zero);
				for (int j = 1; j < stride; ++j)
				{
					bounds.Encapsulate(vertices[indices[i + j]]);
				}
				ret.Add(bounds);
			}
		}

		return ret;
	}

	static public float SplitSegment(Plane plane, Vector3 v0, Vector3 v1)
	{
		return (plane.distance - Vector3.Dot(plane.normal, v0)) / Vector3.Dot(plane.normal, v1 - v0);
	}

	static public bool SideToPlane(Plane plane, Vector3 position)
	{
		return plane.distance - Vector3.Dot(plane.normal, position) <= Mathf.Epsilon;
	}

	static public bool PlaneCoincide(Plane a, Plane b)
	{
		return a.normal == b.normal && Mathf.Approximately(a.distance, b.distance);
	}
}
