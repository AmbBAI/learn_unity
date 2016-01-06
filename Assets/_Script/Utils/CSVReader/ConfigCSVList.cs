using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ConfigCSVList<TConfig> : ConfigBase where TConfig : new() {
	protected List<TConfig> configList = new List<TConfig>();
	protected string keyField = "id";

	public ConfigCSVList()
	{
	}

	public ConfigCSVList(string keyField)
	{
		this.keyField = keyField;
	}

	public override void ParseData(string data)
    {
        var csvData = CSVReader.Read(data);
        configList.Clear();

        Type configType = typeof(TConfig);
        var labels = csvData[0].Keys;

        for (int i = 0; i < csvData.Count; ++i)
        {
            string key = Convert.ToString(csvData[i][keyField]);
            if (string.IsNullOrEmpty(key)) continue;

            TConfig config = new TConfig();
            foreach (var label in labels)
            {
                ParseUtils.ReflectObject<TConfig>(config, label, csvData[i][label]);
            }
            configList.Add(config);
        }
    }


    public virtual TConfig this[int index]
    {
        get
        {
            if (index < 0 || index >= configList.Count)
                return default(TConfig);
            else
                return configList[index];
        }
    }

    public virtual int Count
    {
        get
        {
            return configList.Count;
        }
    }
}
   