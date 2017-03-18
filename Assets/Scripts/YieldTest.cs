using UnityEngine;
using System.Collections;
using System.Collections.Generic;



class MyYieldInstruction
{
	public virtual void Start() { }
	public virtual bool Update() { return true; }
}

class MyWaitForSeconds : MyYieldInstruction
{
	float startTime = 0f;
	float seconds;
	public MyWaitForSeconds(float seconds) { this.seconds = seconds; }
	public override void Start() { base.Start(); startTime = Time.time; }
	public override bool Update()
	{
		if (Time.time - startTime >= seconds)
		{
			return true;
		}
		return false;
	}
}

public class YieldTest : MonoBehaviour {

	List<IEnumerator> routineList = new List<IEnumerator>();
	public void MyStartCoroutine(IEnumerator itor) { routineList.Add(itor); }

	bool MoveNext(IEnumerator routine)
	{
		if (routine == null) return false;
		if (!routine.MoveNext()) return false;
		var myYield = routine.Current as MyWaitForSeconds;
		if (myYield != null) myYield.Start();
		return true;
	}

	void Update () {
		List<IEnumerator> newList = new List<IEnumerator>();
		foreach (var routine in routineList)
		{
			var myItor = routine.Current as MyYieldInstruction;
			if (myItor == null || myItor.Update())
			{
				if (MoveNext(routine)) newList.Add(routine);
            }
			else
			{
				newList.Add(routine);
			}
		}
		routineList = newList;
	}

	IEnumerator _Test()
	{
		Debug.Log(Time.time);
		yield return new MyWaitForSeconds(5f);
		Debug.Log(Time.time);
	}

	[ContextMenu("Test")]
	public void Test()
	{
		MyStartCoroutine(_Test());
	}
}
