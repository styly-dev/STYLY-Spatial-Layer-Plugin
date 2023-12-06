using System.IO;
using Newtonsoft.Json;

namespace Styly.VisionOs.Plugin
{
    public class Metadata
    {
        [JsonProperty("plugin_version")]
        public string PluginVersion { get; set; }
        [JsonProperty("unity_version")]
        public string UnityVersion { get; set; }
        [JsonProperty("build_at")]
        public string BuiltAt { get; set; }
        [JsonProperty("asset_path")]
        public string AssetPath { get; set; }
        [JsonProperty("asset_type")]
        public string AssetType { get; set; }
        [JsonProperty("visual_scripting_version")]
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
                PluginVersion = PackageManagerUtility.Instance.GetPackageVersion(Config.PackageName),
                UnityVersion = UnityEngine.Application.unityVersion,
                BuiltAt = builtAt,
                AssetPath = assetPath,
                AssetType = assetType,
                VisualScriptingVersion = PackageManagerUtility.Instance.GetPackageVersion(Config.VisualScriptingName)
            };

            var metadataJson = JsonConvert.SerializeObject(metadata, Formatting.Indented);

            return metadataJson;        
        }
    }
}