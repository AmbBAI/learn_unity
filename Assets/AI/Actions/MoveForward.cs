using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class MoveForward : RAINAction
{
	Transform _transform;

    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);

		_transform = ai.Body.GetComponent<Transform>();
	}

	public override ActionResult Execute(RAIN.Core.AI ai)
    {
		ai.Motor.MoveTo(_transform.position + _transform.forward);
		return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}