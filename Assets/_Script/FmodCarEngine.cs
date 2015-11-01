using UnityEngine;
using System.Collections;
using FMOD.Studio;

public class FmodCarEngine : MonoBehaviour {

	public FMOD_StudioEventEmitter emitter;
	ParameterInstance rpmParameter;
	ParameterInstance loadParameter;
	[Range(0f, 10000f)]
	public float rpm;
	[Range(-1f, 1f)]
	public float load;

	void Update()
	{
		if (rpmParameter == null)
		{
			rpmParameter = emitter.getParameter("RPM");
			loadParameter = emitter.getParameter("Load");
		}

		rpmParameter.setValue(rpm);
		loadParameter.setValue(load);
	}
}
