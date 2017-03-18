using UnityEngine;
using System.Collections;

public class DistanceToEye : MonoBehaviour {

	[System.Serializable]
	public class Object
	{
		public Transform trans;
		public float distance;
	};

	public Object[] objs;

	void Update () {
		if (Camera.main.orthographic)
		{
			var cameraPos = Camera.main.transform.position;
			var cameraForward = Camera.main.transform.forward;

			for (int i = 0; i < objs.Length; ++i)
			{
				if (objs[i].trans == null) continue;
				objs[i].distance = Vector3.Dot(objs[i].trans.position - cameraPos, cameraForward);
			}
		}
		else
		{
			var cameraPos = Camera.main.transform.position;
			for (int i = 0; i < objs.Length; ++i)
			{
				if (objs[i].trans == null) continue;
				objs[i].distance = (objs[i].trans.position - cameraPos).magnitude;
			}
		}
	}
}
