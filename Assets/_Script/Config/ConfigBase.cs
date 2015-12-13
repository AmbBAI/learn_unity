using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class ConfigBase {

	static public TValue CSVGet<TValue>(object csvObject, TValue defaultValue)
	{
		try
		{
			return (TValue)System.Convert.ChangeType(csvObject, typeof(TValue));
		}
		catch
		{
			return defaultValue;
		}
	}

	static public bool ReflectObject<TObject>(TObject obj, string name, object value)
	{
		Type objType = typeof(TObject);
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		PropertyInfo pinfo = objType.GetProperty(name, flags);
		if (pinfo != null)
		{
			Type type = pinfo.PropertyType;
			pinfo.SetValue(obj, ReflectParseType(value, type), null);
		}
		else
		{
			FieldInfo finfo = objType.GetField(name, flags);
			if (finfo != null)
			{
				Type type = finfo.FieldType;
				finfo.SetValue(obj, ReflectParseType(value, type));
			}
			else return false;
		}
		return true;
	}

	static Char[] splitChar = new Char[] { ';', '|', '#', '~' };
	static public string[] StringSplit(string str)
	{
		if (string.IsNullOrEmpty(str)) return new string[0];
		return str.Split(splitChar);
	}

	static public object ReflectParseType(object value, Type type)
	{
		if (type == typeof(int[]))
		{
			string[] args = StringSplit(value.ToString());
			return Array.ConvertAll<string, int>(args, x => int.Parse(x));
		}
		if (type == typeof(float[]))
		{
			string[] args = StringSplit(value.ToString());
			return Array.ConvertAll<string, float>(args, x => float.Parse(x));
		}

		try
		{
			return Convert.ChangeType(value, type);
		}
		catch (Exception e)
		{
			return null;
		}
	}

	public virtual void ParseData(string data) { }
}
