using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyTest : MonoBehaviour {

	Rigidbody _rigidbody;
	public Vector3 force;
	public Vector3 velocity;

	// Use this for initialization
	void Start () {
		_rigidbody = GetComponent<Rigidbody>();
	}

	[ContextMenu("AddForce")]
	public void AddForce()
	{
		_rigidbody.AddForce(force);
	}

	[ContextMenu("AddVelocity")]
	public void AddVelocity()
	{
		_rigidbody.velocity += velocity;
	}
}
