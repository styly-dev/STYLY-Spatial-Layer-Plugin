using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System;
using System.IO;

public class FindGameObjectsInAllScenes : EditorWindow
{
    string searchName = "(Upload as Prefab)";
    string searchFolder = "Assets/Samples-STYLY";
    Vector2 scroll;
    List<(string scenePath, GameObject target, string objectPath)> foundList = new List<(string, GameObject, string)>();

    [MenuItem("Tools/Find GameObjects In All Scenes")]
    public static void ShowWindow()
    {
        GetWindow<FindGameObjectsInAllScenes>("Find GameObjects In All Scenes");
    }

    void OnGUI()
    {
        searchFolder = EditorGUILayout.TextField("Search Folder", searchFolder);
        searchName = EditorGUILayout.TextField("Search Name", searchName);

        if (GUILayout.Button("Find in All Scenes & Export Prefab"))
        {
            FindInAllScenes(searchFolder, searchName);
        }

        EditorGUILayout.LabelField("Results:", EditorStyles.boldLabel);
        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var result in foundList)
        {
            EditorGUILayout.LabelField($"{result.scenePath} : {result.objectPath}");
        }

        EditorGUILayout.EndScrollView();
    }

    void FindInAllScenes(string folder, string targetName)
    {
        foundList.Clear();

        var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { folder });
        string currentScene = EditorSceneManager.GetActiveScene().path;
        bool sceneSaved = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        foreach (var guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);

            // 指定フォルダ配下のみ
            if (!scenePath.StartsWith(folder))
                continue;

            var openedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.scene == openedScene &&
                    obj.name.IndexOf(targetName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    string objPath = GetFullPath(obj.transform);
                    foundList.Add((scenePath, obj, objPath));
                    Debug.Log($"Found: {objPath} in Scene: {scenePath}", obj);

                    // --- PrefabとしてAssets/Tempに保存 ---
                    SaveAsPrefab(obj);
                }
            }
        }

        if (!string.IsNullOrEmpty(currentScene) && sceneSaved)
        {
            EditorSceneManager.OpenScene(currentScene, OpenSceneMode.Single);
        }
    }

    static string GetFullPath(Transform transform)
    {
        string path = "/" + transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = "/" + transform.name + path;
        }
        return path;
    }

    static void SaveAsPrefab(GameObject obj)
    {
        string tempFolder = "Assets/Temp";
        if (!AssetDatabase.IsValidFolder(tempFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Temp");
        }

        string prefabName = $"{obj.name}.prefab";
        string prefabPath = Path.Combine(tempFolder, prefabName).Replace("\\", "/");

        // 親がいる場合は一時的にコピーしてroot扱いで保存
        GameObject rootObject = obj;
        GameObject tempRoot = null;
        if (obj.transform.parent != null)
        {
            tempRoot = GameObject.Instantiate(obj);
            tempRoot.name = obj.name;
            rootObject = tempRoot;
        }

        PrefabUtility.SaveAsPrefabAssetAndConnect(rootObject, prefabPath, InteractionMode.UserAction);

        if (tempRoot != null)
        {
            GameObject.DestroyImmediate(tempRoot);
        }

        Debug.Log($"Saved prefab: {prefabPath}");
    }
}