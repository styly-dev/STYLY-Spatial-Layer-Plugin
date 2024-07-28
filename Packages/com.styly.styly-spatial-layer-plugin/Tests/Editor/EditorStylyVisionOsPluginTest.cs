using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using File = UnityEngine.Windows.File;

namespace Styly.VisionOs.Plugin
{
    public class EditorStylyVisionOsPluginTest
    {
        [SetUp]
        public void Setup()
        {
            if (Directory.Exists(Config.OutputPath))
            {
                Directory.Delete(Config.OutputPath,true);
            }
        }
        
        [Test]
        public void SwitchPlatformToVisionOs()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
            Assert.That(EditorUserBuildSettings.activeBuildTarget, Is.EqualTo(BuildTarget.StandaloneOSX));
            
            var assetBundleUtility = new AssetBundleUtility();
            var result = assetBundleUtility.SwitchPlatform(BuildTarget.VisionOS);
            
            Assert.That(result, Is.True);
            Assert.That(EditorUserBuildSettings.activeBuildTarget, Is.EqualTo(BuildTarget.VisionOS));
        }

        [Test]
        public void CreateThumbnail()
        {
            var assetPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Prefab/Cube.prefab";
            var filename = "thumbnail";
            var outputPath = Path.Combine(Config.OutputPath,"thumbnail", $"{filename}.png");
            CreateThumbnailUtility.MakeThumbnail(assetPath, outputPath );

            Assert.That(File.Exists(outputPath), Is.True );
        }

        [Test]
        public void ExportBackupFile()
        {
            var assetPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Prefab/Cube.prefab";
            var outputPath = Path.Combine(Config.OutputPath);
            ExportBackupFileUtility.Export(assetPath, outputPath );

            Assert.That(File.Exists(Path.Combine(outputPath,"backup.unitypackage")), Is.True );
            Assert.That(File.Exists(Path.Combine(outputPath,"manifest.json")), Is.True );
            
        }

        [Test]
        public void BuildAssetBundle()
        {
            var assetPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Prefab/Cube.prefab";
            var assetBundleUtility = new AssetBundleUtility();
            var result = assetBundleUtility.SwitchPlatform(BuildTarget.VisionOS);
            Assert.That(result, Is.True);

            var filename = "assetbundle";
            var outputPath = Path.Combine(Config.OutputPath,"VisionOS");
            result = assetBundleUtility.Build(filename, assetPath, outputPath, BuildTarget.VisionOS);
            
            Assert.That(result, Is.True);
            Assert.That(File.Exists( Path.Combine(outputPath, filename)), Is.True );
        }

        [Test]
        public void CreateBuildInfo()
        {
            var assetPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Prefab/Cube.prefab";
            var date = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:sszzz");
            var json = MetadataUtility.CreateBuildInfoJson(assetPath, date);
            Debug.Log(json);
            
            Assert.That(json, Is.Not.Empty);

            var buildInfo = JsonConvert.DeserializeObject<BuildInfo>(json);

            Assert.That(buildInfo.PluginVersion, Is.Not.Null);
            Assert.That(buildInfo.AssetPath, Is.EqualTo(assetPath));
            Assert.That(buildInfo.BuiltAt, Is.EqualTo(date));
            Assert.That(buildInfo.AssetType, Is.EqualTo("Prefab"));
            Assert.That(buildInfo.VisualScriptingVersion, Is.EqualTo("1.9.4"));
        }

        [Test]
        public void LoadAssetBundle()
        {
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
            var bundlePath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/AssetBundle/Cube";
            var assetBundleUtility = new AssetBundleUtility();
            var gameObject = assetBundleUtility.LoadFromAssetBundle(bundlePath);
            
            Assert.That(gameObject, Is.Not.Null);
            Assert.That(gameObject.name, Is.EqualTo("Cube"));
        }
        
        
        [Test]
        public void CreateMetaJson()
        {
            var full_jsonPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Json/full-Value.json";
            var expectedJson = System.IO.File.ReadAllText(full_jsonPath).TrimEnd();
            var assetPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Prefab/PrefabWithVariables_Value.prefab";
            
            var result = MetadataUtility.CreateMetaJson(assetPath);
            
            Debug.Log(result);
            Assert.That(result, Is.Not.Empty);

            Assert.That(CompareJsonStructure(result,expectedJson ), Is.True);
        }

        
        public static bool CompareJsonStructure(string json1, string json2)
        {
            var obj1 = JObject.Parse(json1);
            var obj2 = JObject.Parse(json2);

            return JToken.DeepEquals(
                NormalizeJsonStructure(obj1),
                NormalizeJsonStructure(obj2)
            );
        }

        private static JToken NormalizeJsonStructure(JToken token)
        {
            if (token.Type == JTokenType.Object)
            {
                return new JObject(
                    token.Children<JProperty>()
                        .OrderBy(prop => prop.Name)
                        .Select(prop => new JProperty(prop.Name, NormalizeJsonStructure(prop.Value)))
                );
            }
            else if (token.Type == JTokenType.Array)
            {
                return new JArray(
                    token.Children().Select(NormalizeJsonStructure)
                );
            }
            else
            {
                return new JValue((string)null);
            }
        }
        
        //
        // // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // // `yield return null;` to skip a frame.
        // [UnityTest]
        // public IEnumerator EditorVisionOsPluginTestWithEnumeratorPasses()
        // {
        //     // Use the Assert class to test conditions.
        //     // Use yield to skip a frame.
        //     yield return null;
        // }
    }
}