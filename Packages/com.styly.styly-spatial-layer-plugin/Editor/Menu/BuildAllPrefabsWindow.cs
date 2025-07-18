using UnityEngine;
using UnityEditor;
using System.IO;
using Styly.SpatialLayer.Plugin;

public class BuildAllPrefabsMenu
{
    // BuildTargetは必要なものに応じて調整
    static BuildTarget[] buildTargets = new[] { BuildTarget.VisionOS, BuildTarget.Android };

    [MenuItem("Tools/Build All Prefab in Assets/Temp")]
    public static void BuildAllPrefabs()
    {
        string tempDir = "Assets/Temp";
        if (!Directory.Exists(tempDir))
        {
            EditorUtility.DisplayDialog("Error", "Assets/Temp folder does not exist.", "OK");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { tempDir });
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("Info", "No prefabs found in Assets/Temp.", "OK");
            return;
        }

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            try
            {
                AssetMenu.BuildContent(path, buildTargets);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Build failed for {path}: {ex.Message}");
            }
        }

        EditorUtility.DisplayDialog("Build Complete", "All prefabs in Assets/Temp have been built.", "OK");
    }
}