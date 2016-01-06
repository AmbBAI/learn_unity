using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using LitJson;

public static class FileUtils{

	static public byte[] secretKey = null;
	static public byte[] StringToBytes(string data)
	{
		return Encoding.UTF8.GetBytes(data);
	}
	static public string BytesToString(byte[] bytes)
	{
		return Encoding.UTF8.GetString(bytes);
	}

	static public int EncryptData(int value)
	{
		if (secretKey == null || secretKey.Length <= 0) return value;

		int keyLength = secretKey.Length;
		int key = Convert.ToInt32(secretKey[0 % keyLength]);
		key = (key << 8) + Convert.ToInt32(secretKey[1 % keyLength]);
		key = (key << 8) + Convert.ToInt32(secretKey[2 % keyLength]);
		key = (key << 8) + Convert.ToInt32(secretKey[3 % keyLength]);
		return key ^ value;
	}

	static public int DecryptData(int value)
	{
		return EncryptData(value);
	}

	static public void EncryptData(byte[] bytes)
	{
		if (secretKey == null || secretKey.Length <= 0) return;

		int keyLength = secretKey.Length;
		for (int i = 0; i < bytes.Length; ++i)
		{
			bytes[i] ^= secretKey[i % keyLength];
		}
	}

	static public void DecryptData(byte[] bytes)
	{
		EncryptData(bytes);
	}

	static public string GetMD5(byte[] bytes)
	{
		using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
		{
			if (md5 == null)
			{
				return null;
			}
			byte[] md5Bytes = md5.ComputeHash(bytes, 0, bytes.Length);
			string md5String = BitConverter.ToString(md5Bytes);
			md5String = md5String.Replace("-", "");
			md5String = md5String.ToLower();
			return md5String;
		}
	}

	static public string GetMD5(string str)
	{
		return GetMD5(StringToBytes(str));
	}

	static public string LoadStringFromFile(string filePath)
	{
		FileInfo fileInfo = new FileInfo(filePath);
		if (fileInfo.Exists == false)
		{
			return null;
		}

		StreamReader streamReader;
		streamReader = fileInfo.OpenText();
		if (streamReader == null)
		{
			return null;
		}

		string data = streamReader.ReadToEnd();
		streamReader.Close();
		return data;
	}

	static public void SaveStringToFile(string filePath, string data)
	{
		FileInfo fileInfo = new FileInfo(filePath);
		StreamWriter streamWriter;

		streamWriter = fileInfo.CreateText();
		if (streamWriter == null)
		{
			return;
		}

		streamWriter.Write(data);
		streamWriter.Flush();
		streamWriter.Close();
	}

}
