using UnityEngine;
using UnityEditor;

namespace Styly.VisionOs.Plugin
{
    /// <summary>
    /// Set Preload Audio Data for all AudioClips in the project
    /// </summary>
    public class SetPreloadAudioData
    {
        public static void SetPreloadDataOfAllAudioClips()
        {
            // Get all AudioClip assets
            string[] audioGuids = AssetDatabase.FindAssets("t:AudioClip");
            foreach (string guid in audioGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);

                // Get the AudioImporter for the AudioClip
                AudioImporter importer = (AudioImporter)AssetImporter.GetAtPath(path);
                if (importer != null)
                {
                    // Set the Preload Audio Data to true
                    AudioImporterSampleSettings sampleSettings = importer.defaultSampleSettings;
                    if (!sampleSettings.loadType.HasFlag(AudioClipLoadType.Streaming) && !sampleSettings.preloadAudioData)
                    {
                        sampleSettings.preloadAudioData = true;
                        importer.defaultSampleSettings = sampleSettings;
                        importer.SaveAndReimport();
                        Debug.Log("Preload Audio Data has been set for " + path);
                    }

                    // Check for override settings and change Preload Audio Data for each platform
                    foreach (BuildTargetGroup targetGroup in System.Enum.GetValues(typeof(BuildTargetGroup)))
                    {
                        if (targetGroup == BuildTargetGroup.Unknown) continue;
                        AudioImporterSampleSettings platformSettings = importer.GetOverrideSampleSettings(targetGroup);
                        if (!platformSettings.loadType.HasFlag(AudioClipLoadType.Streaming) && !platformSettings.preloadAudioData)
                        {
                            platformSettings.preloadAudioData = true;
                            importer.SetOverrideSampleSettings(targetGroup, platformSettings);
                            importer.SaveAndReimport();
                            Debug.Log("Preload Audio Data has been set for " + path + " on platform " + targetGroup);
                        }
                    }
                }
            }
        }
    }
}