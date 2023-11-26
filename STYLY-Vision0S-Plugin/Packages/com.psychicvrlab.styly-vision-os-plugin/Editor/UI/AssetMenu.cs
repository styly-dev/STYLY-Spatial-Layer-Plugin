using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Styly.VisionOs.Plugin
{
    public class AssetMenu
    {
        private static bool isProcessing;
        
        [MenuItem(@"Assets/STYLY/Build Content File", false, 10000)]
        private static void BuildContent()
        {
            isProcessing = true;

            var assetBundleUtility = new AssetBundleUtility();
            
            var assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
            Debug.Log($"Selected asset:{assetPath}");

            if (!IsBuildTargetType(assetPath))
            {
                Debug.LogError("Selected asset is not prefab");
                return;
            }
            
            if (Directory.Exists(Config.OutputPath))
            {
                Directory.Delete(Config.OutputPath,true);
            }
            var outputPath = Path.Combine(Config.OutputPath,DateTime.Now.ToString("yyyyMMddHHmmss"));
            Debug.Log(outputPath);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            
            CreateThumbnailUtility.MakeThumbnail(Path.Combine(outputPath, "thumbnail.png") , assetPath);
            
            // Todo:Export Unitypackage
            
            SetPlatformRequiresReadableAssets(true);
            assetBundleUtility.SwitchPlatform(BuildTarget.VisionOS);
            
            // Todo:Build Asset Bundle
            
            // Todo:Create meta.json
            
            // Todo:Compress to Zip file
            
            // Todo:Open Export Folder
            
            // Todo:Open Browser
            
            isProcessing = false;
        }

        private static bool IsBuildTargetType(string path)
        {
            return Path.GetExtension(path).ToLower() == ".prefab";
        }

        private static void SetPlatformRequiresReadableAssets(bool val)
        {
            var path = "ProjectSettings/ProjectSettings.asset";
            var asset = AssetDatabase.LoadAllAssetsAtPath(path).FirstOrDefault();
        
            if (asset == null)
            {
                Debug.LogError("Failed to load ProjectSettings.asset");
                return;
            }
        
            var serializedObject = new SerializedObject(asset);
            var property = serializedObject.FindProperty("platformRequiresReadableAssets");
        
            if (property == null)
            {
                Debug.LogError("Failed to find platformRequiresReadableAssets");
                return;
            }
        
            property.boolValue = val;
            serializedObject.ApplyModifiedProperties();
            Debug.Log("Set platformRequiresReadableAssets to " + property.boolValue);
        }
    }

}
