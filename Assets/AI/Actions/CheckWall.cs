using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;

[RAINAction]
public class CheckWall : RAINAction
{
	Transform _transform;

	public Expression radius;
	public Expression distance;
	public Expression direction;
	public Expression isLocal;
	public Expression isInvertResult;

	public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);

		_transform = ai.Body.GetComponent<Transform>();
	}

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		float _radius = radius.Evaluate<float>(Time.deltaTime, ai.WorkingMemory);
        float _distance = distance.Evaluate<float>(Time.deltaTime, ai.WorkingMemory);
		Vector3 _direction = direction.Evaluate<Vector3>(Time.deltaTime, ai.WorkingMemory);
		bool _isLocal = isLocal.Evaluate<bool>(Time.deltaTime, ai.WorkingMemory);
		bool _isInvertResult = isInvertResult.Evaluate<bool>(Time.deltaTime, ai.WorkingMemory);

		Ray ray = new Ray();
		ray.origin = _transform.position;
		if (_isLocal)
			ray.direction = _transform.localToWorldMatrix.MultiplyVector(_direction.normalized);
		else ray.direction = _direction.normalized;
		bool rayResult = Physics.SphereCast(ray, _radius, _distance - _radius, LayerMask.GetMask("Scene"));
		Debug.DrawRay(ray.origin, ray.direction * _distance, rayResult ? Color.green : Color.red, 0.1f);
		if (_isInvertResult) rayResult = !rayResult;
		return rayResult ? ActionResult.SUCCESS : ActionResult.FAILURE;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}