using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class MoveForward : RAINAction
{
	public RAIN.Representation.Expression moveDistance;
	Transform _transform;

    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);

		_transform = ai.Body.GetComponent<Transform>();
	}

	public override ActionResult Execute(RAIN.Core.AI ai)
    {
		float _moveDistance = moveDistance.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory);
		ai.Motor.MoveTo(_transform.position + _transform.forward * _moveDistance);
		return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}