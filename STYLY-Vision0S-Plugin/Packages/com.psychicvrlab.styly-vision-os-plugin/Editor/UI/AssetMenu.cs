using System.Linq;
using PlasticPipe.PlasticProtocol.Messages.Serialization;
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

            var path = AssetDatabase.GetAssetPath(Selection.objects[0]);
            Debug.Log($"Selected asset:{path}");

            if (!IsBuildTargetType(path))
            {
                Debug.LogError("Selected asset is not prefab");
                return;
            }

            SetPlatformRequiresReadableAssets(true);
            
            // Todo:Change Platform to Vision OS
            
            // Todo:Create Thumbnail
            
            // Todo:Export Unitypackage
            
            // Todo:Build Asset Bundle
            
            // Todo:Create meta.json
            
            // Todo:Compress to Zip file
            
            // Todo:Open Export Folder
            
            isProcessing = false;
        }

        private static bool IsBuildTargetType(string path)
        {
            return false;
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
