using UnityEngine;
using UnityEditor;
using System.IO;

public static class DeployPackage
{
    private const string PackageName = "EventAggregator";
    private const string ShownFolderName = "Samples";
    private const string HiddenFolderName = "Samples~";
    private const string SampleMetaFile = "Samples.meta";

    [MenuItem("Deploy/Show Samples Folder")]
    private static void ShowSamples()
    {
        var oldPath = $"{Application.dataPath}/{PackageName}/{HiddenFolderName}";
        if (!Directory.Exists(oldPath))
        {
            Debug.Log("The Samples folder is already shown");
            return;
        }

        var newPath = $"{Application.dataPath}/{PackageName}/{ShownFolderName}";
        Directory.Move(oldPath, newPath);
        AssetDatabase.Refresh();
        Debug.Log("Show Samples folder");
    }

    [MenuItem("Deploy/Hide Samples Folder")]
    private static void HideSamples()
    {
        var oldPath = $"{Application.dataPath}/{PackageName}/{ShownFolderName}";
        if (!Directory.Exists(oldPath))
        {
            Debug.Log("The Samples folder is already hidden");
            return;
        }

        var metaFilePath = $"{Application.dataPath}/{PackageName}/{SampleMetaFile}";
        if (File.Exists(metaFilePath))
        {
            File.Delete(metaFilePath);
        }

        var newPath = $"{Application.dataPath}/{PackageName}/{HiddenFolderName}";
        Directory.Move(oldPath, newPath);
        AssetDatabase.Refresh();
        Debug.Log("Hide Samples folder");
    }
}