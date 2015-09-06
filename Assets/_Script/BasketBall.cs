using UnityEngine;
using System.Collections;

public class BasketBall : MonoBehaviour {

	Rigidbody _rigidbody;
	public Transform targetTrans;
	Transform _transform;

	Vector3 virtualTarget;

	void Start () {
		_transform = transform;
		_rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if (_transform.position.y < -1f)
		{
			_rigidbody.isKinematic = true;
			_transform.position = Vector3.zero;
			_transform.rotation = Quaternion.identity;
		}
	}

	[ContextMenu("Shot")]
	public void Shot()
	{
		if (!Application.isPlaying) return;

		Vector3 velocity = CalcThrowVelocity();
		Debug.Log(velocity);
		_rigidbody.isKinematic = false;
		_rigidbody.velocity = velocity;
	}

	Vector3 CalcThrowVelocity()
	{
		Vector3 delta = targetTrans.position - _transform.position;
		virtualTarget = targetTrans.position - delta * 0.1f;
		float deltaY = virtualTarget.y - targetTrans.position.y;
		if (deltaY < 0f) deltaY = -deltaY;
		if (deltaY < 0.5f) deltaY = 0.5f;
		virtualTarget.y = targetTrans.position.y + deltaY;
		Debug.Log(virtualTarget);

		delta = virtualTarget - _transform.position;
		float t = Random.Range(0.9f, 1f);
		return (delta - Physics.gravity * (0.5f * t * t)) / t;
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere(virtualTarget, 0.2f);
	}
}
