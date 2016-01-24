using UnityEngine;
using System.Collections;
using System.IO;

public class SaveMatCap : MonoBehaviour {

	public Camera front;
	public Camera back;

	void Start()
	{
		SavePNG(front, "matcap_f");
	}

	void SavePNG(Camera cam, string name)
	{
		cam.Render();
		Texture2D saveTex = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.ARGB32, false);
		RenderTexture.active = cam.targetTexture;
		saveTex.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
		saveTex.Apply();
		var bytes = saveTex.EncodeToPNG();

		string path = string.Format("Assets/Resources/{0}.png", name);
		FileStream fsw = File.Create(path);

		using (BinaryWriter writer = new BinaryWriter(fsw))
		{
			writer.Write(bytes, 0, bytes.Length);
			writer.Close();
		}

		fsw.Close();
	}
}
