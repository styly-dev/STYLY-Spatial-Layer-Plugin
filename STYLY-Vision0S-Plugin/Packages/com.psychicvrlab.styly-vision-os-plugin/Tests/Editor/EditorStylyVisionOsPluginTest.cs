using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Styly.VisionOs.Plugin
{
    public class EditorStylyVisionOsPluginTest
    {
        // A Test behaves as an ordinary method
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