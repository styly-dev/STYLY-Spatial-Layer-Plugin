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
        private static readonly string ThumbnailFileName = "thumbnail.png";
        private static readonly string VisionOsDirectoryName = "VisionOS";
        private static readonly string MetaFileName = "meta.json";
        private static readonly string ParameterFileName = "parameter.json";
        private static readonly string AssetBundleFileName = "assetbundle";
        private static readonly string BackupDirectoryName = "Backup";

        [MenuItem(@"Assets/STYLY/Build prefab", false, 10000)]
        private static void BuildContent()
        {
            isProcessing = true;

            var assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
            Debug.Log($"Selected asset:{assetPath}");

            if (!IsBuildTargetType(assetPath))
            {
                Debug.LogError("Selected asset is not prefab");
                return;
            }

            var outputPath = PrepareOutputDirectory();
            
            CreateThumbnailUtility.MakeThumbnail(assetPath, Path.Combine(outputPath, ThumbnailFileName));
            ExportBackupFileUtility.Export(assetPath, Path.Combine(outputPath, BackupDirectoryName));
            BuildAssetBundle(assetPath, Path.Combine(outputPath, VisionOsDirectoryName));
            GenerateMetadata(assetPath, Path.Combine(outputPath, MetaFileName ));
            
            ZipFile.CreateFromDirectory(outputPath, $"{outputPath}.styly");
            EditorUtility.RevealInFinder( Config.OutputPath );
            
            Directory.Delete(outputPath, true);
            
            var uri = new Uri(Config.UploadPage);
            Application.OpenURL(uri.AbsoluteUri);
            
            isProcessing = false;
        }

        private static string PrepareOutputDirectory()
        {
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

            return outputPath;
        }
        
        private static void BuildAssetBundle(string assetPath, string outputPath)
        {
            SetPlatformRequiresReadableAssets(true);
            var assetBundleUtility = new AssetBundleUtility();
            assetBundleUtility.SwitchPlatform(BuildTarget.VisionOS);
            //ARBuildPreprocess.ARBuildPreprocessBuild(BuildTarget.VisionOS);
            assetBundleUtility.Build(AssetBundleFileName, assetPath, outputPath, BuildTarget.VisionOS);
            File.Delete(Path.Combine(outputPath, VisionOsDirectoryName));
            File.Delete(Path.Combine(outputPath, $"{VisionOsDirectoryName}.manifest"));
        }

        private static void GenerateMetadata(string assetPath, string outputPath)
        {
            var metadata = MetadataUtility.CreateMetaJson(assetPath);
            File.WriteAllText(outputPath, metadata);
        }
        
        private static void GenerateParameter(string assetPath, string outputPath)
        {
            GameObject targetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            
            var parameter = VisualScriptingParameterUtility.GetParameterDefinitionJson(targetObj);
            File.WriteAllText(outputPath, parameter);
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
