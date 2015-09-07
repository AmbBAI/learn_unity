using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class WaitMoveForward : RAINAction
{
	Vector3 lastPos;

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		if (ai.Motor.IsAtMoveTarget) return ActionResult.SUCCESS;
		ai.Motor.Move();
		return ActionResult.RUNNING;
    }

}