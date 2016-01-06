using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ConfigCSVDictionary<TKey, TConfig> : ConfigBase where TConfig : new() {
    protected Dictionary<TKey, TConfig> configMap = new Dictionary<TKey, TConfig>();
	protected string keyField = "id";

	public ConfigCSVDictionary()
	{
	}

	public ConfigCSVDictionary(string keyField)
	{
		this.keyField = keyField;
	}

	public override void ParseData(string data)
	{
        var csvData = CSVReader.Read(data);

        var labels = csvData[0].Keys;
        configMap.Clear();

        for (int i = 0; i < csvData.Count; ++i)
        {
			var keyCsvObj = csvData[i][keyField];
			string key = Convert.ToString(keyCsvObj);
            if (string.IsNullOrEmpty(key)) continue;
            TKey configKey = ParseUtils.CSVGet<TKey>(keyCsvObj, default(TKey));

            TConfig config = new TConfig();
            foreach (var label in labels)
            {
                ParseUtils.ReflectObject<TConfig>(config, label, csvData[i][label]);
            }
            configMap.Add(configKey, config);
        }
    }

    public virtual TConfig this[TKey key]
    {
        get
        {
            TConfig config = default(TConfig);
            configMap.TryGetValue(key, out config);
            return config;
        }
    }

}
