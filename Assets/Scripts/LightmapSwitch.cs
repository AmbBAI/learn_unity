using UnityEngine;
using System.Collections;

public class LightmapSwitch : MonoBehaviour {

    [System.Serializable]
    public class LightMapTexture
    {
        public Texture2D near;
        public Texture2D far;
    }

    public LightMapTexture[] lightMap1;
    public LightMapTexture[] lightMap2;
    public Transform rendererRoot;

    [ContextMenu("SwitchLightMap1")]
    public void SwitchLightMap1()
    {
        LightmapData[] lightMap = new LightmapData[lightMap1.Length];
        for (int i=0; i<lightMap.Length; ++i)
        {
            lightMap[i] = new LightmapData();
            lightMap[i].lightmapDir = lightMap1[i].near;
            lightMap[i].lightmapLight = lightMap1[i].far;
        }
        LightmapSettings.lightmaps = lightMap;

        var rendererList = rendererRoot.GetComponentsInChildren<Renderer>();
        for (int i=0; i<rendererList.Length; ++i)
        {
            rendererList[i].lightmapIndex = 0;
        }
    }

    [ContextMenu("SwitchLightMap2")]
    public void SwitchLightMap2()
    {
        LightmapData[] lightMap = new LightmapData[lightMap2.Length];
        for (int i = 0; i < lightMap.Length; ++i)
        {
            lightMap[i] = new LightmapData();
            lightMap[i].lightmapDir = lightMap2[i].near;
            lightMap[i].lightmapLight = lightMap2[i].far;
        }
        LightmapSettings.lightmaps = lightMap;

        var rendererList = rendererRoot.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rendererList.Length; ++i)
        {
            //rendererList[i].lightmapIndex = 0;
            //rendererList[i].lightmapScaleOffset
        }
    }

    [ContextMenu("SaveRendererLightMapData")]
    public void GetRendererLightMapData()
    {
        var rendererList = rendererRoot.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rendererList.Length; ++i)
        {
            Debug.Log(rendererList[i].name);
            Debug.Log(rendererList[i].lightmapIndex);
            Debug.Log(rendererList[i].lightmapScaleOffset);
        }
    }
}
