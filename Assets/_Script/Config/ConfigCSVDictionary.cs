using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ConfigCSVDictionary<TKey, TConfig> : ConfigBase where TConfig : new() {
    protected Dictionary<TKey, TConfig> configMap = new Dictionary<TKey, TConfig>();

	public override void ParseData(string data)
	{
        var csvData = CSVReader.Read(data);

        var labels = csvData[0].Keys;
        configMap.Clear();

        for (int i = 0; i < csvData.Count; ++i)
        {
            string key = Convert.ToString(csvData[i]["key"]);
            if (string.IsNullOrEmpty(key)) continue;
            TKey configKey = CSVGet<TKey>(csvData[i]["key"], default(TKey));

            TConfig config = new TConfig();
            foreach (var label in labels)
            {
                ReflectObject<TConfig>(config, label, csvData[i][label]);
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
