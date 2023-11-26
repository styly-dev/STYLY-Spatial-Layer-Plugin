using System.IO;
using UnityEditor;
using UnityEngine;


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
//         public void Migrate(GameObject obj)
//         {
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

