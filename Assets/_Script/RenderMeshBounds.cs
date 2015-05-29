using UnityEngine;
using System.Collections;

public class RenderMeshBounds : MonoBehaviour {

	void Start()
	{
		BoundsRenderer r = BoundsRenderer.Instance;

		var meshFs = GetComponentsInChildren<MeshFilter>();
		foreach (var meshF in meshFs)
		{
			r.DrawBounds(meshF.mesh.bounds, Color.green);
		}
	}
}
