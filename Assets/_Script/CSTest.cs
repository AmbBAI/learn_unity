using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class CSTest : MonoBehaviour {


    IEnumerator WWWYield()
    {
        for (int i = 0; i < 10; ++i)
        {
            WWW www = new WWW("www.baidu.com");
            yield return www;
            Debug.Log(www.text);
        }
    }

    Rect rect1 = new Rect(0, 0, 500, 20);
    Rect rect2 = new Rect(0, 20, 500, 20);

    void OnGUI()
	{
		GUI.Label(rect1, add(1, 2).ToString());

        if (GUI.Button(rect2, "WWW"))
        {
            StartCoroutine(WWWYield());
        }
	}

	[DllImport("cpp_plugin")]
	private static extern int add(int a, int b);
}
