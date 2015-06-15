using UnityEngine;
using System.Collections;

public class NavMeshTest : MonoBehaviour {

	public NavMeshAgent agent;
	public Animation anim;

	void Update () {

		if (!agent.hasPath) anim.CrossFade("idle");
		else anim.CrossFade("run");
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo))
			{
				agent.SetDestination(hitInfo.point);
			}
		}
	}
}
