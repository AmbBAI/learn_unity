using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINDecision]
public class RevertResult : RAINDecision
{

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		if (_children.Count <= 0) return ActionResult.SUCCESS;
		ActionResult tResult = _children[0].Run(ai);
		if (tResult == ActionResult.SUCCESS) return ActionResult.FAILURE;
		if (tResult == ActionResult.FAILURE) return ActionResult.SUCCESS;
		return tResult;
    }

}