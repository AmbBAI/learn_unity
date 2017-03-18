using UnityEngine;
using System.Collections;

public class GoalCollider : MonoBehaviour {

	public event System.Action<GoalCollider> OnTriggerEnterEvent;
	public event System.Action<GoalCollider> OnTriggerStayEvent;
	public event System.Action<GoalCollider> OnTriggerExitEvent;

	void OnTriggerEnter(Collider c)
	{
		if (OnTriggerEnterEvent != null) OnTriggerEnterEvent(this);
	}

	void OnTriggerStay(Collider c)
	{
		if (OnTriggerStayEvent != null) OnTriggerStayEvent(this);
	}

	void OnTriggerExit(Collider c)
	{
		if (OnTriggerExitEvent != null) OnTriggerExitEvent(this);
	}
}
