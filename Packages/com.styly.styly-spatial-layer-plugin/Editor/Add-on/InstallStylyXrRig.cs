using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ToDo: Check the STYLY supported version of STYLY-XR-Rig

namespace Styly.VisionOs.Plugin
{
    public class InstallStylyXrRig : MonoBehaviour
    {
#if USE_STYLY_XR_RIG
#else
        // Show the menu item only when STYLY-XR-Rig is not installed
        private static readonly string packageName = "com.styly.styly-xr-rig";

        // Add a menu item in the hierarchy context menu
        [MenuItem("GameObject/XR/STYLY-XR-Rig")]
        static void InstallStylyXrRigFromRightClickMenu(MenuCommand menuCommand)
        {
            // Prompt the user to install the package
            if (EditorUtility.DisplayDialog("Install STYLY-XR-Rig", "Do you want to install STYLY-XR-Rig and dependent packages with samples?", "Yes", "No"))
            {
                PackageManagerUtility.AddUnityPackage(packageName);
            }
        }
#endif

    }
}
