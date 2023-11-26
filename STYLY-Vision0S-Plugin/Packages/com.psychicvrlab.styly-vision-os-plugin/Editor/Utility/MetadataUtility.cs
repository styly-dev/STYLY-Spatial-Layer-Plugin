using System.IO;
using Newtonsoft.Json;

namespace Styly.VisionOs.Plugin
{
    public class Metadata
    {
        public string PluginVersion { get; set; }
        public string UnityVersion { get; set; }
        public string BuiltAt { get; set; }
        public string AssetPath { get; set; }
        public string AssetType { get; set; }
        public string VisualScriptingVersion { get; set; }

    }
    
    public class MetadataUtility
    {
        public static string CreateMetadataJson(string assetPath, string builtAt)
        {
            var ext = Path.GetExtension(assetPath);
            string assetType = "Prefab";
            if (ext == "unity")
            {
                assetType = "Scene";
            }
            
            Metadata metadata = new Metadata
            {
                PluginVersion = PackageManagerUtility.Instance.GetPackageVersion("com.psychicvrlab.styly-vision-os-plugin"),
                UnityVersion = UnityEngine.Application.unityVersion,
                BuiltAt = builtAt,
                AssetPath = assetPath,
                AssetType = assetType,
                VisualScriptingVersion = PackageManagerUtility.Instance.GetPackageVersion("com.unity.visualscripting")
            };

            var metadataJson = JsonConvert.SerializeObject(metadata, Formatting.Indented);

            return metadataJson;        
        }
    }
}