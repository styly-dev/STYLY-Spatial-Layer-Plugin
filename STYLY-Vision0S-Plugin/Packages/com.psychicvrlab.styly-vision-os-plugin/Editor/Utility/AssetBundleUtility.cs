using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Styly.VisionOs.Plugin
{
    /// <summary>
    /// Asset Bundle Utility
    /// </summary>
    public class AssetBundleUtility
    {
        public bool SwitchPlatform(BuildTarget buildTarget)
        {
            if (buildTarget == BuildTarget.VisionOS)
            { 
                return EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.VisionOS, BuildTarget.VisionOS);
            }
            return false;
        }

         public AssetBundleManifest Build(string filename, string sourcePath, string destDirectory, BuildTarget buildTarget)
         {
             if (!Directory.Exists(destDirectory))
             {
                 Directory.CreateDirectory(destDirectory);
             }

             var outputFilePath = Path.Combine(destDirectory, filename);
             if (File.Exists( outputFilePath))
             {
                 File.Delete(outputFilePath);
             }
             var outputManifestFilePath = $"{outputFilePath}.manifest";
             if (File.Exists( outputManifestFilePath))
             {
                 File.Delete(outputManifestFilePath);
             }

             AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
             buildMap[0].assetBundleName = filename;
             buildMap[0].assetNames = new string[] { sourcePath };
             var buildResult = BuildPipeline.BuildAssetBundles(destDirectory, buildMap, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);

             return buildResult;
         }

//
//         /// <summary>
//         /// ビルドターゲットからプラットフォーム名を取得する
//         /// </summary>
//         /// <param name="platform">ビルドターゲット</param>
//         /// <returns>プラットフォーム名</returns>
//         public string GetPlatformName(BuildTarget platform)
//         {
//             switch (platform)
//             {
//                 case BuildTarget.Android:
//                     return "Android";
//                 case BuildTarget.iOS:
//                     return "iOS";
//                 case BuildTarget.WebGL:
//                     return "WebGL";
//                 case BuildTarget.WSAPlayer:
//                     return "UWP";
//                 case BuildTarget.StandaloneWindows64:
//                     return "Windows";
//                 case BuildTarget.StandaloneOSX:
//                     return "OSX";
//                 case BuildTarget.VisionOS:
//                     return "VOS";
//                 default:
//                     return null;
//             }
//         }
//
//         /// <summary>
//         /// ランタイムプラットフォームからビルドターゲットを取得する
//         /// </summary>
//         /// <param name="platform">ランタイムプラットフォーム</param>
//         /// <returns>ビルドターゲット</returns>
//         public BuildTarget GetBuildTarget(RuntimePlatform platform)
//         {
//             Debug.Log("GetBuildTarget");
//
//             switch (platform)
//             {
//                 case RuntimePlatform.Android:
//                     return BuildTarget.Android;
//                 case RuntimePlatform.IPhonePlayer:
//                     return BuildTarget.iOS;
//                 case RuntimePlatform.WebGLPlayer:
//                     return BuildTarget.WebGL;
//                 case RuntimePlatform.WSAPlayerX86:
//                     return BuildTarget.WSAPlayer;
//                 case RuntimePlatform.WindowsPlayer:
//                 case RuntimePlatform.WindowsEditor:
//                     return BuildTarget.StandaloneWindows64;
//                 case RuntimePlatform.OSXPlayer:
//                 case RuntimePlatform.OSXEditor:
//                     return BuildTarget.StandaloneOSX;
//                 default:
//                     return BuildTarget.StandaloneWindows64;
//             }
//         }
//
//         /// <summary>
//         /// ビルドターゲットからランタイムプラットフォームを取得する
//         /// </summary>
//         /// <param name="buildTarget">ビルドターゲット</param>
//         /// <returns>ランタイムプラットフォーム</returns>
//         public RuntimePlatform GetRuntimePlatform(BuildTarget buildTarget)
//         {
//             RuntimePlatform platform = RuntimePlatform.WindowsPlayer;
//             switch (buildTarget)
//             {
//                 case BuildTarget.Android:
//                     platform = RuntimePlatform.Android;
//                     break;
//                 case BuildTarget.StandaloneWindows64:
//                     platform = RuntimePlatform.WindowsPlayer;
//                     break;
//                 case BuildTarget.iOS:
//                     platform = RuntimePlatform.IPhonePlayer;
//                     break;
//
//                 case BuildTarget.StandaloneOSX:
//                     platform = RuntimePlatform.OSXPlayer;
//                     break;
//                 case BuildTarget.WebGL:
//                     platform = RuntimePlatform.WebGLPlayer;
//                     break;
//             }
//
//             return platform;
//         }
//
//         private const string STYLY_BUILDED_ASSET_PATH_DATA = "Assets/styly_temp/BuildedAssetPathData.asset";
//         // public BuildedAssetPathData GetBuildedAssetData()
//         // {
//         //     var buildedAssetPathData = AssetDatabase.LoadAssetAtPath<BuildedAssetPathData>(STYLY_BUILDED_ASSET_PATH_DATA);
//         //     if (buildedAssetPathData == null)
//         //     {
//         //         buildedAssetPathData = ClearBuildedAssetPathData();
//         //     }
//         //
//         //     return buildedAssetPathData;
//         // }
//         //
//         // public void SaveBuildedAssetPathData(BuildedAssetPathData _buildedAssetPathData)
//         // {
//         //     var buildedAssetPathData = AssetDatabase.LoadAssetAtPath<BuildedAssetPathData>(STYLY_BUILDED_ASSET_PATH_DATA);
//         //     buildedAssetPathData = _buildedAssetPathData;
//         //     EditorUtility.SetDirty(buildedAssetPathData);
//         //     //保存する
//         //     AssetDatabase.SaveAssets();
//         //     AssetDatabase.Refresh();
//         // }
//         //
//         // public BuildedAssetPathData ClearBuildedAssetPathData()
//         // {
//         //     Debug.Log("new");
//         //     var buildedAssetPathData = ScriptableObject.CreateInstance<BuildedAssetPathData>();
//         //
//         //     if (!Directory.Exists(Path.GetDirectoryName(STYLY_BUILDED_ASSET_PATH_DATA)))
//         //     {
//         //         Directory.CreateDirectory(Path.GetDirectoryName(STYLY_BUILDED_ASSET_PATH_DATA));
//         //     }
//         //     AssetDatabase.CreateAsset(buildedAssetPathData, STYLY_BUILDED_ASSET_PATH_DATA);
//         //     AssetDatabase.SaveAssets();
//         //     AssetDatabase.Refresh();
//         //
//         //     return buildedAssetPathData;
//         // }
//         //
//         // /// <summary>
//         // /// Prefabがビルド済みかどうかBuildedAssetPathDataから判定する。
//         // /// </summary>
//         // /// <param name="prefabPath"></param>
//         // /// <param name="buildTarget"></param>
//         // /// <returns>true: Already builded. false:Need to build.</returns>
//         // public bool CheckPrefabAlreadyBuilded(string prefabPath, BuildTarget buildTarget)
//         // {
//         //     Debug.Log("CheckPrefabAlreadyBuilded:" + prefabPath);
//         //
//         //     // AssetBundleのパスが記録されていなければビルド必要
//         //     var assetBundlePath = GetAssetBundlePath(prefabPath, buildTarget);
//         //     if (assetBundlePath == null)
//         //     {
//         //         Debug.Log("Need to build.");
//         //         return false;
//         //     }
//         //
//         //     // ファイルの存在チェック
//         //     // 存在しなければビルド必要
//         //     Debug.Log("Exist is:" + File.Exists(assetBundlePath) + " path:" + assetBundlePath);
//         //     if (!File.Exists(assetBundlePath))
//         //     {
//         //         Debug.Log("need build:assetBundle is not Exitsts");
//         //         return false;
//         //     }
//         //
//         //     return true;
//         // }
//         //
//         // public string GetAssetBundlePath(string prefabPath, BuildTarget buildTarget)
//         // {
//         //     var buildedAssetData = GetBuildedAssetData();
//         //     var buildedData = buildedAssetData.GetData(prefabPath);
//         //
//         //     // 対象プラットフォームのビルドがあるか確認する。
//         //     string assetBundlePath = Path.Combine(OutputPath, "STYLY_ASSET", GetPlatformName(buildTarget));
//         //     string val;
//         //     if (!buildedData.TryGetValue(BuildedAssetPathData.GUID_KEY, out val))
//         //     {
//         //         return null;
//         //     }
//         //     assetBundlePath = Path.Combine(assetBundlePath, val);
//         //
//         //     return assetBundlePath;
//         // }
//         //
//         // public string GetGuidFromBuildedAssetData(string prefabPath)
//         // {
//         //     var buildedAssetData = GetBuildedAssetData();
//         //     var buildedData = buildedAssetData.GetData(prefabPath);
//         //
//         //     string guidString;
//         //     if (buildedData.TryGetValue(BuildedAssetPathData.GUID_KEY, out guidString))
//         //     {
//         //         return guidString;
//         //     }
//         //     else
//         //     {
//         //         return null;
//         //     }
//         // }
//
//         public GameObject LoadFromAssetBundle(string path, out bool isSceneAsset)
//         {
//             isSceneAsset = false;
//             Resources.UnloadUnusedAssets();
//             AssetBundle bundle = AssetBundle.LoadFromFile(path);
//
//             if (!bundle.isStreamedSceneAssetBundle)
//             {
//                 var prefab = bundle.LoadAllAssets<GameObject>()[0];
//                 var go = GameObject.Instantiate(prefab.gameObject);
//                 Migrate(go);
//                 return go;
//             }
//             else
//             {
//                 if (EditorApplication.isPlaying)
//                 {
//                     SceneManager.LoadScene(Path.GetFileNameWithoutExtension(path), LoadSceneMode.Additive);
//                 }
//                 else
//                 {
//                     bundle.Unload(false);
//                     isSceneAsset = true;
//                     return null;
//                 }
//             }
//
//             return null;
//         }
//
//         GameObject LoadPrefabFromAssetBundle(AssetBundle bundle)
//         {
//             Debug.Log("bundle!");
//             var prefab = bundle.LoadAllAssets<GameObject>()[0];
//             Debug.Log("bundle2:" + prefab.name);
//
//             var go = GameObject.Instantiate(prefab.gameObject);
//             return go;
//         }
//         
//         
//         /// <summary>
//         /// GameObject配下（子、孫を含む）の各MaterialおよびTerrainのシェーダーを更新する。
//         /// </summary>
//         /// <param name="obj"></param>
//         public void Migrate(GameObject obj)
//         {
//             // 通常アセットのRenderer処理
//             var renderers = obj.GetComponentsInChildren<Renderer>(true);
//             foreach (var renderer in renderers)
//             {
// #if UNITY_EDITOR                
//                 foreach (var mat in renderer.sharedMaterials)
//                 {
//                     MigrateShaderInMaterial(mat);
//                 }
// #else
//                 foreach (var mat in renderer.materials)
//                 {
//                     MigrateShaderInMaterial(mat);
//                 }
// #endif
//             }
//         }
//
//         // マテリアル内のシェーダーを置換
//         private void MigrateShaderInMaterial(Material mat)
//         {
//             if (mat.shader == null)
//             {
//                 return;
//             }
//             var shaderName = mat.shader.name;
//
//             Shader replaceShader = null;
//             replaceShader = Shader.Find(shaderName);
//         
//             if (replaceShader != null)
//             {
//                 mat.shader = replaceShader;
//             }
         // }
    }
}

