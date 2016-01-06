using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using LitJson;

public static class ParseUtils
{
	#region LitJson
	static public string PrettyWriteJson(object obj)
	{
		JsonWriter writer = new JsonWriter();
		writer.PrettyPrint = true;
		JsonMapper.ToJson(obj, writer);
		return writer.ToString();
	}

	static public void RegisterJsonMapperImporter()
	{
		JsonMapper.RegisterImporter<JsonData, Vector3>(
			delegate (JsonData data)
			{
				Vector3 ret = Vector3.zero;
				if (data.Count > 0) ret.x = System.Convert.ToSingle(data[0].GetReal());
				if (data.Count > 1) ret.y = System.Convert.ToSingle(data[1].GetReal());
				if (data.Count > 2) ret.z = System.Convert.ToSingle(data[2].GetReal());
				return ret;
			}
		);
		JsonMapper.RegisterImporter<JsonData, Vector4>(
			delegate (JsonData data)
			{
				Vector4 ret = Vector4.zero;
				if (data.Count > 0) ret.x = System.Convert.ToSingle(data[0].GetReal());
				if (data.Count > 1) ret.y = System.Convert.ToSingle(data[1].GetReal());
				if (data.Count > 2) ret.z = System.Convert.ToSingle(data[2].GetReal());
				if (data.Count > 3) ret.w = System.Convert.ToSingle(data[3].GetReal());
				return ret;
			}
		);
	}

	static public void RegisterJsonMapperExporter()
	{
		JsonMapper.RegisterExporter<Vector3>(
			delegate (Vector3 obj, JsonWriter writer)
			{
				writer.WriteArrayStart();
				writer.Write(obj.x);
				writer.Write(obj.y);
				writer.Write(obj.z);
				writer.WriteArrayEnd();
			}
		);

		JsonMapper.RegisterExporter<Vector4>(
			delegate (Vector4 obj, JsonWriter writer)
			{
				writer.WriteArrayStart();
				writer.Write(obj.x);
				writer.Write(obj.y);
				writer.Write(obj.z);
				writer.Write(obj.w);
				writer.WriteArrayEnd();
			}
		);
	}
	#endregion

	#region String List
	static Char[] defaultSplitChar = new Char[] { ';', '|', '#', '~' };
	static public string[] StringSplit(string str)
	{
        if (string.IsNullOrEmpty(str)) return new string[0];
		return str.Split(defaultSplitChar);
	}

    static public Type SLParse<Type>(string[] args, int idx, Type defaultValue)
    {
        if (args.Length <= idx) return defaultValue;
        if (string.IsNullOrEmpty(args[idx])) return defaultValue;

        try
        {
            return (Type)System.Convert.ChangeType(args[idx], typeof(Type));
        }
        catch
        {
            return defaultValue;
        }
    }

	static public string ParseString(string[] args, int idx, string defaultValue = "")
	{
		if (args.Length <= idx) return defaultValue;
		return args[idx];
	}

	static public DateTime ParseDateTime(string[] args, int idx, DateTime defaultValue)
	{
		if (args.Length <= idx) return defaultValue;
		try
		{
			return DateTime.Parse(args[idx]);
		}
		catch
		{
			return defaultValue;
		}
	}

	static public KeyValuePair<TKey, TValue> ParseKeyValuePair<TKey, TValue>(string data)
	{
		string[] args = ParseUtils.StringSplit(data);
		if (args.Length < 2) return default(KeyValuePair<TKey, TValue>);
		return new KeyValuePair<TKey, TValue>(
				(TKey)Convert.ChangeType(args[0], typeof(TKey)),
				(TValue)Convert.ChangeType(args[1], typeof(TValue))
			);
	}

	static public List<KeyValuePair<TKey, TValue>> ParseKeyValuePairList<TKey, TValue>(string data)
	{
		List<KeyValuePair<TKey, TValue>> ret = new List<KeyValuePair<TKey, TValue>>();
		string[] args = ParseUtils.StringSplit(data);
		for (int i = 0; i + 1 < args.Length; i += 2)
		{
			ret.Add(new KeyValuePair<TKey, TValue>(
				(TKey)Convert.ChangeType(args[i], typeof(TKey)),
				(TValue)Convert.ChangeType(args[i + 1], typeof(TValue))
			));
		}
		return ret;
	}
	#endregion

	#region CSV
	static public TObject CSVGet<TObject>(object csvObject, TObject defaultValue)
    {
        try
        {
            return (TObject)System.Convert.ChangeType(csvObject, typeof(TObject));
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

	static public TObject DeepCopy<TObject>(TObject source) where TObject : class
	{
		if (!typeof(TObject).IsSerializable)
		{
			throw new ArgumentException("The type must be serializable.", "source");
		}

		// Don't serialize a null object, simply return the default for that object
		if (System.Object.ReferenceEquals(source, null))
		{
			return default(TObject);
		}

		IFormatter formatter = new BinaryFormatter();
		Stream stream = new MemoryStream();
		using (stream)
		{
			formatter.Serialize(stream, source);
			stream.Seek(0, SeekOrigin.Begin);
			return formatter.Deserialize(stream) as TObject;
		}
	}
	#endregion

	#region Format
	static public string HandleOtherFormats(string format, object arg)
	{
		if (arg is IFormattable)
			return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
		else if (arg != null)
			return arg.ToString();
		else
			return String.Empty;
	}
	#endregion

}