using System.Collections;

public class ConfigBase {

	public virtual void LoadFromLocal(string file)
	{
        if (string.IsNullOrEmpty(file)) return;
		string data = FileUtils.LoadStringFromFile(file);
		ParseData(data);
	}

	public virtual void ParseData(string data) { }
}
