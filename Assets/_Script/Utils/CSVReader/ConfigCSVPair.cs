using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ConfigCSVPair<TConfig> : ConfigBase where TConfig : new(){
    protected TConfig config = new TConfig();
    public TConfig data { get { return config; } }

    public override void ParseData(string data)
	{
		var csvData = CSVReader.Read(data);
		Type configType = typeof(TConfig);
		for (int i = 0; i < csvData.Count; ++i)
		{
			string key = Convert.ToString(csvData[i]["key"]);
			if (string.IsNullOrEmpty(key)) continue;

            ParseUtils.ReflectObject<TConfig>(config, key, csvData[i]["value"]);
		}
	}
}
