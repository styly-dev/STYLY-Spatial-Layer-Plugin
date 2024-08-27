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
        private static readonly string ThumbnailDirName = "Thumbnails";
        private static readonly string VisionOsDirectoryName = "VisionOS";
        private static readonly string MetaFileName = "meta.json";
        private static readonly string ParameterFileName = "parameter.json";
        private static readonly string AssetBundleFileName = "assetbundle";
        private static readonly string BackupDirectoryName = "Backup";

        [MenuItem(@"Assets/STYLY/Build Prefab")]
        private static void BuildContent()
        {
            isProcessing = true;

            var assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
            var assetFileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
            Debug.Log($"Selected asset:{assetPath}");

            if (!IsBuildTargetType(assetPath))
            {
                Debug.LogError("Selected asset is not a Prefab.");
                return;
            }

            var outputPath = PrepareOutputDirectory();

            CreateThumbnailUtility.MakeThumbnail(assetPath, Path.Combine(outputPath, ThumbnailFileName));
            ExportBackupFileUtility.Export(assetPath, Path.Combine(outputPath, BackupDirectoryName));
            bool buildResult = BuildAssetBundle(assetPath, Path.Combine(outputPath, VisionOsDirectoryName));
            if (buildResult == false)
            {
                Directory.Delete(outputPath, true);
                return;
            }
            GenerateMetadata(assetPath, Path.Combine(outputPath, MetaFileName));

            ConpyAssetBundleToHostingDirectory(outputPath, assetFileNameWithoutExtension);
            ConpyThumbnailToHostingDirectory(outputPath, assetFileNameWithoutExtension);
            
            ZipFile.CreateFromDirectory(outputPath, $"{outputPath}_{assetFileNameWithoutExtension}.styly");
            EditorUtility.RevealInFinder(Config.OutputPath);

            Directory.Delete(outputPath, true);

            var uri = new Uri(Config.UploadPage);
            Application.OpenURL(uri.AbsoluteUri);

            isProcessing = false;
        }

        [MenuItem(@"Assets/STYLY/Build Prefab", validate = true, priority = 10000)]
        static bool ValidateBuildContent()
        {
            if (Application.isPlaying) return false;
            if (Selection.objects.Length != 1) return false;
            var assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
            return IsBuildTargetType(assetPath);
        }

        private static string PrepareOutputDirectory()
        {
            var outputPath = Path.Combine(Config.OutputPath, DateTime.Now.ToString("yyyyMMddHHmmss"));
            Debug.Log(outputPath);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            return outputPath;
        }

        private static bool BuildAssetBundle(string assetPath, string outputPath)
        {
#if USE_UNITY_XR_VISIONOS
            EnablePluginProviders.EnableXRPlugin(BuildTargetGroup.VisionOS, typeof(UnityEngine.XR.VisionOS.VisionOSLoader));
#endif
            SetPreloadAudioData.SetPreloadDataOfAllAudioClips();
            SetPlatformRequiresReadableAssets(true);
            var assetBundleUtility = new AssetBundleUtility();
            assetBundleUtility.SwitchPlatform(BuildTarget.VisionOS);
            ARBuildPreprocess.ARBuildPreprocessBuild(BuildTarget.VisionOS);
            var buildResult = assetBundleUtility.Build(AssetBundleFileName, assetPath, outputPath, BuildTarget.VisionOS);
            File.Delete(Path.Combine(outputPath, VisionOsDirectoryName));
            File.Delete(Path.Combine(outputPath, $"{VisionOsDirectoryName}.manifest"));

            return buildResult != null;
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
    
        const string HostingDirectoryName = "html";

        private static void ConpyAssetBundleToHostingDirectory(string outputPath, string assetFileNameWithoutExtension)
        {
            
            var assetBundlePath = Path.Combine(outputPath, VisionOsDirectoryName, AssetBundleFileName);
            var HostingDirPath = Path.Combine(Directory.GetParent(outputPath).ToString(), HostingDirectoryName, VisionOsDirectoryName);
            var assetBundleOutputPath = Path.Combine(HostingDirPath, $"{Path.GetFileName(outputPath)}_{assetFileNameWithoutExtension}");
            
            if (!Directory.Exists(HostingDirPath)) { Directory.CreateDirectory(HostingDirPath);}
            
            File.Copy(assetBundlePath, assetBundleOutputPath);
        }
        private static void ConpyThumbnailToHostingDirectory(string outputPath, string assetFileNameWithoutExtension)
        {
            var thumbnailPath = Path.Combine(outputPath, ThumbnailFileName);
            var HostingDirPath = Path.Combine(Directory.GetParent(outputPath).ToString(), HostingDirectoryName, ThumbnailDirName);
            var thumbnailOutputPath = Path.Combine(HostingDirPath, $"{Path.GetFileName(outputPath)}_{assetFileNameWithoutExtension}.png");
            
            if (!Directory.Exists(HostingDirPath)) { Directory.CreateDirectory(HostingDirPath);}
            
            File.Copy(thumbnailPath, thumbnailOutputPath);
        }
    }

}
