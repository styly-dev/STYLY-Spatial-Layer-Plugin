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
        [Test]
        public void SwitchPlatformToVisionOs()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
            Assert.That(EditorUserBuildSettings.activeBuildTarget, Is.EqualTo(BuildTarget.StandaloneOSX));
            
            var abUtility = new AssetBundleUtility();
            var result = abUtility.SwitchPlatform(BuildTarget.VisionOS);
            
            Assert.That(result, Is.True);
            Assert.That(EditorUserBuildSettings.activeBuildTarget, Is.EqualTo(BuildTarget.VisionOS));
        }

        [Test]
        public void CreateThumbnail()
        {
            var assetPath = "Packages/com.psychicvrlab.styly-vision-os-plugin/Editor/TestData/Prefab/Cube.prefab";
            var filename = "thumbnail";
            var path = Path.Combine(Config.OutputPath,"thumbnail", $"{filename}.png");
            CreateThumbnailUtility.MakeThumbnail(path, assetPath);

            Assert.That(File.Exists(path), Is.True );
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