using UnityEngine;
using System.Collections;

public class GoalChecker : MonoBehaviour {

	public GoalCollider collider0;
	public GoalCollider collider1;

	bool isInCollider0 = false;

	void Start()
	{
		collider0.OnTriggerEnterEvent += delegate (GoalCollider gc) { Debug.Log("gc0 >"); };
        collider0.OnTriggerExitEvent += delegate (GoalCollider gc) { Debug.Log("gc0 <"); };
        collider1.OnTriggerEnterEvent += delegate (GoalCollider gc) { Debug.Log("gc1 >"); };
        collider1.OnTriggerExitEvent += delegate (GoalCollider gc) { Debug.Log("gc1 <"); };

		collider0.OnTriggerEnterEvent += delegate (GoalCollider gc) { isInCollider0 = true; };
		collider0.OnTriggerExitEvent += delegate (GoalCollider gc) { isInCollider0 = false; };
		collider1.OnTriggerEnterEvent += delegate (GoalCollider gc) { if (isInCollider0) Debug.Log("Cheer"); };
    }


}
