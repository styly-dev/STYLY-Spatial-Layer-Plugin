using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Styly.VisionOs.Plugin
{
    public class ARBuildPreprocess
    {
        /// <summary>
        /// PreprocessBuild for XRReferenceImageLibrary into AssetBundle
        /// https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.1/manual/features/image-tracking.html#use-reference-image-libraries-with-assetbundles
        /// </summary>
        /// <param name="buildTarget"></param>
        public static void ARBuildPreprocessBuild(BuildTarget buildTarget)
        {
            UnityEditor.XR.ARSubsystems.ARBuildProcessor.PreprocessBuild(buildTarget);
        }
    }
}
