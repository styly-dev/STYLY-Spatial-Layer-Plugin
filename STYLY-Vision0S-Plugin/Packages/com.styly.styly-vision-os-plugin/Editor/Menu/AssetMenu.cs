using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Styly.VisionOs.Plugin
{
    public class AssetMenu
    {
        private static bool isProcessing;

        [MenuItem(@"Assets/STYLY/Test Variable Utility", false, 10001)]
        private static void TestVariableUtility(){
            VisualScriptingParameterUtility.GetParameterDefinitionJson(Selection.objects[0] as GameObject); 
        }


        
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
            
            CreateThumbnailUtility.MakeThumbnail(assetPath, Path.Combine(outputPath, "thumbnail.png"));
            
            ExportPackageUtility.Export(assetPath, Path.Combine(outputPath, "backup.unitypackage"));
            
            SetPlatformRequiresReadableAssets(true);
            assetBundleUtility.SwitchPlatform(BuildTarget.VisionOS);
            var assetbundleOutputPath = Path.Combine(outputPath, "VisionOS");
            assetBundleUtility.Build("assetbundle", assetPath, assetbundleOutputPath, BuildTarget.VisionOS);
            File.Delete(Path.Combine(assetbundleOutputPath, "VisionOS"));
            File.Delete(Path.Combine(assetbundleOutputPath, "VisionOS.manifest"));
            
            var date = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:sszzz");
            var metadata = MetadataUtility.CreateMetadataJson(assetPath, date);
            var metadataOutputPath = Path.Combine(outputPath, "meta.json");
            File.WriteAllText(metadataOutputPath, metadata);

            GameObject targetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            
            var parameter = VisualScriptingParameterUtility.GetParameterDefinitionJson(targetObj);
            var parameterOutputPath = Path.Combine(outputPath, "parameter.json");
            File.WriteAllText(parameterOutputPath, parameter);
            
            ZipFile.CreateFromDirectory(outputPath, $"{outputPath}.styly");

            EditorUtility.RevealInFinder( Config.OutputPath );
            
            Directory.Delete(outputPath, true);
            
            var uri = new Uri(Config.UploadPage);
            Application.OpenURL(uri.AbsoluteUri);
            
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
