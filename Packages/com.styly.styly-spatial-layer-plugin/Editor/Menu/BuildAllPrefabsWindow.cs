using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Styly.SpatialLayer.Plugin;

public class BuildAllPrefabsWindow : EditorWindow
{
    Vector2 scroll;
    List<string> prefabPaths = new List<string>();
    List<bool> selected;
    BuildTarget[] buildTargets = new[] { BuildTarget.VisionOS, BuildTarget.Android }; // 必要に応じて変更

    [MenuItem("Tools/Build All Prefab in Assets/Temp")]
    public static void ShowWindow()
    {
        GetWindow<BuildAllPrefabsWindow>("Build All Prefab in Assets/Temp");
    }

    void OnEnable()
    {
        RefreshPrefabList();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Refresh Prefab List"))
        {
            RefreshPrefabList();
        }

        EditorGUILayout.LabelField("Select Prefabs to Build:", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        for (int i = 0; i < prefabPaths.Count; i++)
        {
            selected[i] = EditorGUILayout.ToggleLeft(Path.GetFileName(prefabPaths[i]), selected[i]);
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Build Selected Prefabs"))
        {
            BuildSelectedPrefabs();
        }
    }

    void RefreshPrefabList()
    {
        prefabPaths.Clear();
        selected = new List<bool>();

        string tempDir = "Assets/Temp";
        if (!Directory.Exists(tempDir))
        {
            Debug.LogWarning("Assets/Temp folder does not exist.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { tempDir });
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            prefabPaths.Add(path);
            selected.Add(true); // デフォルトで全選択
        }
    }

    void BuildSelectedPrefabs()
    {
        for (int i = 0; i < prefabPaths.Count; i++)
        {
            if (!selected[i])
                continue;

            try
            {
                // 必要に応じてビルドターゲットを変更
                AssetMenu.BuildContent(prefabPaths[i], buildTargets);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Build failed for {prefabPaths[i]}: {ex.Message}");
            }
        }
        EditorUtility.DisplayDialog("Build Complete", "Selected prefabs have been built.", "OK");
    }
}