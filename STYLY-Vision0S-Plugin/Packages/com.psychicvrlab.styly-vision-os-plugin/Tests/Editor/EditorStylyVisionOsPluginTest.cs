using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            var assetPath = "Packages/com.psychicvrlab.styly-vision-os-plugin/Tests/Editor/TestData/Prefab/Cube.prefab";
            var filename = "thumbnail";
            var outputPath = Path.Combine(Config.OutputPath,"thumbnail", $"{filename}.png");
            CreateThumbnailUtility.MakeThumbnail(assetPath, outputPath );

            Assert.That(File.Exists(outputPath), Is.True );
        }

        [Test]
        public void ExportUnitypackage()
        {
            var assetPath = "Packages/com.psychicvrlab.styly-vision-os-plugin/Tests/Editor/TestData/Prefab/Cube.prefab";
            var filename = "backup";
            var outputPath = Path.Combine(Config.OutputPath,"packages", $"{filename}.unitypackage");
            ExportPackageUtility.Export(assetPath, outputPath );

            Assert.That(File.Exists(outputPath), Is.True );
            
        }

        [Test]
        public void BuildAssetBundle()
        {
            var assetPath = "Packages/com.psychicvrlab.styly-vision-os-plugin/Tests/Editor/TestData/Prefab/Cube.prefab";
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
        public void CreateMetadata()
        {
            var assetPath = "Packages/com.psychicvrlab.styly-vision-os-plugin/Tests/Editor/TestData/Prefab/Cube.prefab";
            var date = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:sszzz");
            var json = MetadataUtility.CreateMetadataJson(assetPath, date);
            Debug.Log(json);
            
            Assert.That(json, Is.Not.Empty);
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