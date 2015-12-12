using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class RotateFace : RAINAction
{
	Transform _transform;

	public RAIN.Representation.Expression rotateAngle;
	public RAIN.Representation.Expression isLocal;

	public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);

		_transform = ai.Body.GetComponent<Transform>();
	}

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		Vector3 _rotateAngle = rotateAngle.Evaluate<Vector3>(Time.deltaTime, ai.WorkingMemory);
		bool _isLocal = isLocal.Evaluate<bool>(Time.deltaTime, ai.WorkingMemory);
		Quaternion rotation = new Quaternion();
		rotation.eulerAngles = _rotateAngle;
		if (_isLocal) rotation = _transform.rotation * rotation;
		_transform.rotation = rotation;
		ai.Motor.FaceAt(_transform.position + _transform.forward);
        return ActionResult.SUCCESS;
    }

}