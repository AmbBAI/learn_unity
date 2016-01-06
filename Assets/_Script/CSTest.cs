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


	List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

	int TestFor()
	{
		int ret = 0;
		for (int i=0; i<list.Count; ++i)
		{
			ret += list[i];
		}
		return ret;
	}

	int TestForEach()
	{
		int ret = 0;
		foreach (var i in list)
		{
			ret += i;
		}
		return ret;
	}

	int TestListForEach()
	{
		int ret = 0;
		list.ForEach(i => ret += i);
		return ret;
	}

	int TestWhile()
	{
		int ret = 0;
		var e = list.GetEnumerator();
		while (e.MoveNext())
		{
			ret += e.Current;
		}
		return ret;
	}

	int TestWhileUsing()
	{
		int ret = 0;
		using (var e = list.GetEnumerator())
		{
			while (e.MoveNext())
			{
				ret += e.Current;
			}
		}
		return ret;
	}

	int TestForDelegate()
	{
		int ret = 0;
		Action<int> func = x => ret += x;
		for (int i=0; i< list.Count; ++i)
		{
			func(list[i]);
		}
		return ret;
	}

	void Update()
	{
		TestFor();
		TestForEach();
		TestListForEach();
		TestWhile();
		TestWhileUsing();
		TestForDelegate();
	}

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
