using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {

	Transform trans;
	public float rot = 10f;

	void Start () {
		trans = transform;
	}
	
	void Update () {
		trans.Rotate(Vector3.up, rot * Time.deltaTime);
	}
}
