using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ExportAssetBundles
{

    // Store current texture format for the TextureProcessor.
    //public static TextureImporterFormat textureFormat;

    public static string packPath = "Assets/Resources/";
    public static string exportPath = "Assets/StreamingAssets/";

    static void ExportResource(Object[] objs, string path)
    {
        BuildTarget target =
#if UNITY_ANDROID
 BuildTarget.Android;
#elif UNITY_IPHONE
			BuildTarget.iPhone;
#else
			BuildTarget.StandaloneWindows;
#endif

        BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;

        BuildPipeline.BuildAssetBundle(null, objs, path, options, target);
    }

    [MenuItem("Assets/Build AssetBundle From Selection", false, 2000)]
    static void ExportSelectedResource()
    {
        // Build the resource file from the active selection.
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        //foreach (object asset in selection)
        //{
        //	string assetPath = AssetDatabase.GetAssetPath((UnityEngine.Object)asset);
        //	if (asset is Texture2D)
        //	{
        //		// Force reimport thru TextureProcessor.
        //		AssetDatabase.ImportAsset(assetPath);
        //	}
        //}

        // Bring up save panel.
        string path = EditorUtility.SaveFilePanel("Save Resource", exportPath, "New Resource", "assetbundle");

        if (path.Length != 0)
        {
            ExportResource(selection, path);
        }
        Selection.objects = selection;
    }

    [MenuItem("Assets/Clear Cache", false, 2002)]
    static void ClearCache()
    {
        Caching.CleanCache();
    }

}
