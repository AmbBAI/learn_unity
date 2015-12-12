using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class CheckWall : RAINAction
{
	Transform _transform;

	public RAIN.Representation.Expression distance;
	public RAIN.Representation.Expression direction;
	public RAIN.Representation.Expression isLocal;

	public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);

		_transform = ai.Body.GetComponent<Transform>();
	}

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		float _distance = distance.Evaluate<float>(Time.deltaTime, ai.WorkingMemory);
		Vector3 _direction = direction.Evaluate<Vector3>(Time.deltaTime, ai.WorkingMemory);
		bool _isLocal = isLocal.Evaluate<bool>(Time.deltaTime, ai.WorkingMemory);

		Ray ray = new Ray();
		ray.origin = _transform.position;
		if (_isLocal)
			ray.direction = _transform.localToWorldMatrix.MultiplyVector(_direction.normalized);
		else ray.direction = _direction.normalized;
		bool rayResult = Physics.Raycast(ray, _distance);
		Debug.Log(ray.direction + ",," + rayResult);
		return rayResult ? ActionResult.SUCCESS : ActionResult.FAILURE;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}