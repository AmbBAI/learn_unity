using UnityEngine;
using System.Collections;

public class CameraMatrix : MonoBehaviour {

	public Vector3 forwardIn = Vector3.forward;
	public Vector3 forwardOut;

	void Start () {
		//Camera c = camera;
		//Debug.Log(c.cameraToWorldMatrix);
		//Debug.Log(c.projectionMatrix);
	}

	void Update()
	{
		//transform.forward = forwardIn;
		forwardOut = transform.forward;
	}
}
