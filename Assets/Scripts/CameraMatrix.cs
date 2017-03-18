using UnityEngine;
using System.Collections;

public class CameraMatrix : MonoBehaviour {

	public Vector3 forwardIn = Vector3.forward;
	public Vector3 forwardOut;

    Matrix4x4 viewMatrix;
    Matrix4x4 projMatrix;
    Matrix4x4 gpuProjMatrix;

	void Start () {
		//Camera c = camera;
		//Debug.Log(c.cameraToWorldMatrix);
		//Debug.Log(c.projectionMatrix);
	}

	void Update()
	{
		//transform.forward = forwardIn;
		forwardOut = transform.forward;

        Camera c = Camera.main;
        viewMatrix = c.cameraToWorldMatrix;
        projMatrix = c.projectionMatrix;
        gpuProjMatrix = GL.GetGPUProjectionMatrix(projMatrix, false);
	}
}
