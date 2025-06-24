using System.Collections.Generic;
using System.Linq;

namespace Styly.SpatialLayer.Plugin
{
    public class StylySystemInfo
    {
        public const string URL_KEY = "URL";
        
        private static readonly Dictionary<string, string> systemInfo = new();
        public static IReadOnlyDictionary<string, string> SystemInfo => systemInfo;

        public static string GetInfo(string key)
        {
            return systemInfo.ContainsKey(key) ? systemInfo[key] : string.Empty;
        }

        public static List<string> GetInfoNameList()
        {
            return systemInfo.Keys.ToList();
        }

        internal static void SetInfoInternal(string key, string value)
        {
            systemInfo[key] = value;
        }
        
        internal static void ClearInfoInternal()
        {
            systemInfo.Clear();
        }

    }

    public class StylySystemInfoManager
    {
        public static void SetInfo(string key, string value)
        {
            StylySystemInfo.SetInfoInternal(key, value);
        }

        public static void Clear()
        {
            StylySystemInfo.ClearInfoInternal();
        }
    }
}