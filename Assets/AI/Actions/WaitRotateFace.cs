using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class WaitRotateFace : RAINAction
{
    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		if (ai.Motor.IsFacingFaceTarget) return ActionResult.SUCCESS;
		ai.Motor.Face();
		return ActionResult.RUNNING;
    }

}