using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	Transform trans = null;
	public float moveScale = 5f;
	public float rotateScale = 0.5f;

	void Start () {
		trans = transform;
	}
	
	void Update () {
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) trans.Translate(trans.forward * moveScale, Space.World);
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) trans.Translate(-trans.forward * moveScale, Space.World);
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) trans.Translate(-trans.right * moveScale, Space.World);
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) trans.Translate(trans.right * moveScale, Space.World);
	}

	void OnGUI()
	{
		Event e = Event.current;
		if (e.type == EventType.mouseDrag)
		{
			Vector2 moveDelta = Event.current.delta;
			trans.Rotate(new Vector3(moveDelta.y, moveDelta.x, 0) * rotateScale, Space.Self);
			Vector3 rotation = trans.localEulerAngles;
			rotation.z = 0f;
			trans.localEulerAngles = rotation;
		}
	}
}
