using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

            var metadata = JsonConvert.DeserializeObject<BuildInfo>(json);

            Assert.That(metadata.PluginVersion, Is.EqualTo("0.0.1"));
            Assert.That(metadata.AssetPath, Is.EqualTo(assetPath));
            Assert.That(metadata.BuiltAt, Is.EqualTo(date));
            Assert.That(metadata.AssetType, Is.EqualTo("Prefab"));
            Assert.That(metadata.VisualScriptingVersion, Is.EqualTo("1.9.1"));
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