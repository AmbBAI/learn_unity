using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyTest : MonoBehaviour {

	Rigidbody _rigidbody;
	public float moveK = 5f;
	public float jumpK = 10f;

	void Start () {
		_rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		Camera currentCamera = Camera.current;
		if (currentCamera == null) return;
		Transform cameraTrans = currentCamera.transform;
		Vector3 force = Vector3.zero;
		if (Input.GetKey(KeyCode.UpArrow)) force += cameraTrans.forward;
		if (Input.GetKey(KeyCode.DownArrow)) force -= cameraTrans.forward;
		if (Input.GetKey(KeyCode.LeftArrow)) force -= cameraTrans.right;
		if (Input.GetKey(KeyCode.RightArrow)) force += cameraTrans.right;

		_rigidbody.velocity += (force.normalized) * Time.deltaTime * moveK;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("?????");
			_rigidbody.velocity += cameraTrans.up * jumpK;
		}
	}
}
