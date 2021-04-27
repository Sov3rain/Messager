using UnityEngine;
using UnityEditor;
using System.IO;

public static class DeployPackage
{
    private const string PackageName = "EventAggregator";
    private const string PackagePath = "Assets/EventAggregator";
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

    [MenuItem("Deploy/Create Unity Package")]
    private static void CreateUnityPackage()
    {
        string assetPath = Application.dataPath;
        string projectPath = Path.GetFullPath(Path.Combine(assetPath, @"..\"));
        string buildPath = Path.Combine(projectPath, @"Builds");
        string jsonManifestPath = Path.Combine(assetPath, "EventAggregator/package.json");

        if (!Directory.Exists(buildPath))
        {
            Directory.CreateDirectory(buildPath);
        }

        string json = File.ReadAllText(jsonManifestPath);
        Manifest manifest = JsonUtility.FromJson<Manifest>(json);
        string version = manifest.version;
        string fullPath = Path.Combine(buildPath, $@"{PackageName}_v{version}.unitypackage");

        AssetDatabase.ExportPackage(
            assetPathName: PackagePath,
            fileName: fullPath,
            ExportPackageOptions.Recurse | ExportPackageOptions.Interactive
        );
        AssetDatabase.Refresh();
    }

    public class Manifest
    {
        public string version;
    }
}