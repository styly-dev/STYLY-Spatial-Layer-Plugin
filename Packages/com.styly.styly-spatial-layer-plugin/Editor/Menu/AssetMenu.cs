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
        private static readonly string AndroidDirectoryName = "Android";
        private static readonly string MetaFileName = "meta.json";
        private static readonly string ParameterFileName = "parameter.json";
        private static readonly string AssetBundleFileName = "assetbundle";
        private static readonly string BackupDirectoryName = "Backup";

#if !STYLY_EXPERIMENTAL
        [MenuItem(@"Assets/STYLY/Build Prefab")]
        private static void BuildVisionOsContent()
        {
            BuildContent(new[]{BuildTarget.VisionOS});
        }
#endif
#if STYLY_EXPERIMENTAL
        [MenuItem(@"Assets/STYLY/Build Prefab for visionOS", false, 100)]
        private static void BuildVisionOsContent()
        {
            BuildContent(new[]{BuildTarget.VisionOS});
        }
        
        [MenuItem(@"Assets/STYLY/Build Prefab for Android", false, 101)]
        private static void BuildAndroidContent()
        {
            BuildContent(new[] { BuildTarget.Android });
        }
        
        [MenuItem(@"Assets/STYLY/Build Prefab for visionOS and Android", false, 102)]
        private static void BuildVisionOSandAndroidContent()
        {
            BuildContent(new[] { BuildTarget.VisionOS, BuildTarget.Android });
        }
#endif
        
        private static void BuildContent(BuildTarget[] buildTargets)
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

            foreach (var buildTarget in buildTargets)
            {
                var directoryName = buildTarget switch
                {
                    BuildTarget.VisionOS => VisionOsDirectoryName,
                    BuildTarget.Android => AndroidDirectoryName,
                    _ => "UnknownPlatform"
                };

                bool buildResult = BuildAssetBundle(assetPath, Path.Combine(outputPath, directoryName), buildTarget);
                if (buildResult == false)
                {
                    Directory.Delete(outputPath, true);
                    return;
                }
            }
            
            GenerateMetadata(assetPath, Path.Combine(outputPath, MetaFileName));

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

        private static bool BuildAssetBundle(string assetPath, string outputPath, BuildTarget buildTarget)
        {
#if USE_UNITY_XR_VISIONOS
            EnablePluginProviders.EnableXRPlugin(BuildTargetGroup.VisionOS, typeof(UnityEngine.XR.VisionOS.VisionOSLoader));
#endif
            SetPreloadAudioData.SetPreloadDataOfAllAudioClips();
#if UNITY_VISIONOS
            SetPlatformRequiresReadableAssets(true);
#endif
            var assetBundleUtility = new AssetBundleUtility();
            assetBundleUtility.SwitchPlatform(buildTarget);
            ARBuildPreprocess.ARBuildPreprocessBuild(buildTarget);
            var buildResult = assetBundleUtility.Build(AssetBundleFileName, assetPath, outputPath, buildTarget);
            var directoryName = buildTarget switch
            {
                BuildTarget.VisionOS => VisionOsDirectoryName,
                BuildTarget.Android => AndroidDirectoryName,
                _ => "UnknownPlatform"
            };
            
            File.Delete(Path.Combine(outputPath, directoryName));
            File.Delete(Path.Combine(outputPath, $"{directoryName}.manifest"));

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
    }

}
