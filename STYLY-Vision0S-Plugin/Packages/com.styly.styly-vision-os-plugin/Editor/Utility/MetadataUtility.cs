using System.IO;
using Newtonsoft.Json;

namespace Styly.VisionOs.Plugin
{
    public class BuildInfo
    {
        [JsonProperty("plugin_version")]
        public string PluginVersion { get; set; }
        [JsonProperty("unity_version")]
        public string UnityVersion { get; set; }
        [JsonProperty("built_at")]
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
        public static string CreateBuildInfoJson(string assetPath, string builtAt)
        {
            var ext = Path.GetExtension(assetPath);
            string assetType = "Prefab";
            if (ext == "unity")
            {
                assetType = "Scene";
            }
            
            BuildInfo buildInfo = new BuildInfo
            {
                PluginVersion = PackageManagerUtility.Instance.GetPackageVersion(Config.PackageName),
                UnityVersion = UnityEngine.Application.unityVersion,
                BuiltAt = builtAt,
                AssetPath = assetPath,
                AssetType = assetType,
                VisualScriptingVersion = PackageManagerUtility.Instance.GetPackageVersion(Config.VisualScriptingName)
            };

            var metadataJson = JsonConvert.SerializeObject(buildInfo, Formatting.Indented);

            return metadataJson;        
        }
    }
}