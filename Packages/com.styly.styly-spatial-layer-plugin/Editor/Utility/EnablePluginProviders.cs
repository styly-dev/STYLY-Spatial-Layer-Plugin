using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;

namespace Styly.VisionOs.Plugin
{
    /// <summary>
    /// Enable XR Plugin Providers for a specific build target group.
    /// </summary>
    /// <remarks>
    /// Usage: EnablePluginProviders.EnableXRPlugin(BuildTargetGroup.VisionOS, typeof(UnityEngine.XR.VisionOS.VisionOSLoader));
    /// </remarks>
    public class EnablePluginProviders : MonoBehaviour
    {
        public static void EnableXRPlugin(BuildTargetGroup buildTargetGroup, Type loaderType)
        {
            // Get the current XRGeneralSettings instance
            XRGeneralSettings xrGeneralSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            if (xrGeneralSettings == null)
            {
                Debug.LogError("XRGeneralSettings is null. Make sure XR Plugin Management is installed and configured.");
                return;
            }

            XRManagerSettings xrManagerSettings = xrGeneralSettings.AssignedSettings;
            if (xrManagerSettings == null)
            {
                Debug.LogError("XRManagerSettings is null. Make sure XR Plugin Management is installed and configured.");
                return;
            }

            // Enable the specified loader if it's not already enabled
            if (!xrManagerSettings.activeLoaders.Any(loader => loader.GetType() == loaderType))
            {
                var xrLoader = ScriptableObject.CreateInstance(loaderType) as XRLoader;
                xrManagerSettings.TryAddLoader(xrLoader);

                // Set the initialization mode to OnDemand or Automatic as needed
                xrManagerSettings.automaticLoading = true;
                xrManagerSettings.automaticRunning = true;

                // Save the changes
                EditorUtility.SetDirty(xrGeneralSettings);
                EditorUtility.SetDirty(xrManagerSettings);
                AssetDatabase.SaveAssets();

                Debug.Log($"{loaderType.Name} has been enabled successfully for {buildTargetGroup}.");
            }
        }
    }
}