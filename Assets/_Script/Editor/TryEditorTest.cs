using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

class CsvConfig
{
	public int key = 0;
	public int intValue = 0;
	public int[] intArray = null;
	public float floatValue = 0f;
	public float[] floatArray = null;
	public string strValue = string.Empty;
}

public class TryEditorTest {

	[Test]
	public void TestUnityWaitForSeconds()
	{
		var src = Object.FindObjectOfType<EditorTestMonoBehaviour>();
		src.StartCoroutine(TestUnityWaitForSecondsRoutine());
	}

	IEnumerator TestUnityWaitForSecondsRoutine()
	{
		float start = Time.time;
		yield return new WaitForSeconds(5f);
		Assert.GreaterOrEqual(Time.time - start, 5f);
		Assert.Less(Time.time - start, 5f);
	}

	[Test]
	public void TestCsvPairRead()
	{
		var asset = Resources.Load<TextAsset>("Config/pair_test");
		Assert.IsNotNull(asset);

		ConfigCSVPair<CsvConfig> csv = new ConfigCSVPair<CsvConfig>();
		csv.ParseData(asset.text);

		Assert.IsNotNull(csv.data);
		Assert.AreEqual(csv.data.intValue, 10);
		Assert.AreEqual(csv.data.intArray.Length, 4);
		Assert.AreEqual(csv.data.intArray[0], 1);
		Assert.AreEqual(csv.data.floatValue, 1.234f);
		Assert.AreEqual(csv.data.floatArray[2], 2.3f);
		Assert.AreEqual(csv.data.strValue, "Hello 中文");
		//Assert.AreEqual(csv.data.strValue, "Hello 中文,x");
	}

	[Test]
	public void TestCsvListRead()
	{
		var asset = Resources.Load<TextAsset>("Config/list_test");
		Assert.IsNotNull(asset);

		ConfigCSVList<CsvConfig> csv = new ConfigCSVList<CsvConfig>();
		csv.ParseData(asset.text);

		Assert.AreEqual(csv.Count, 4);
		Assert.AreEqual(csv[0].intArray[0], 2);
		Assert.Catch<System.IndexOutOfRangeException>(delegate() { float val = csv[0].intArray[1]; });
		//Assert.Catch<System.OutOfMemoryException>(delegate() { float val = csv[0].intArray[1]; });
		Assert.AreEqual(csv[0].strValue, "1,1|2#3~4");
		Assert.IsEmpty(csv[2].intArray);
		//Assert.IsNull(csv[2].intArray);
		Assert.IsEmpty(csv[3].floatArray);
	}
}
