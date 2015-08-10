using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ConvertibleTest
{
	object obj;
	public ConvertibleTest(object obj) { this.obj = obj; }

	public object ToType(Type conversionType)
	{
		Debug.Log(conversionType);
		return new List<int> { 0 };
	}
}

public class CSTest : MonoBehaviour {

	void Start()
	{
		ConvertibleTest ct = new ConvertibleTest((object)10);
		object ret = Convert.ChangeType(ct, typeof(List<int>));
		Debug.Log(ret);
	}
}
