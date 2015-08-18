using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class CSTest : MonoBehaviour {

	void OnGUI()
	{
		GUI.Label(new Rect(0, 0, 500, 500), add(1, 2).ToString());
	}

	[DllImport("cpp_plugin")]
	private static extern int add(int a, int b);
}
