using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class CSTest : MonoBehaviour {

	void Start()
	{
		Debug.Log(add(1, 2));
	}

	[DllImport("cpp_plugin")]
	private static extern int add(int a, int b);
}
