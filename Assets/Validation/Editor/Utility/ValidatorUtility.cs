using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public class ValidatorUtility
    {
        public static string GetGameObjectPath(GameObject obj)
        {
            var wkObj = obj;
            string path = wkObj.name;
            while (wkObj.transform.parent != null)
            {
                wkObj = wkObj.transform.parent.gameObject;
                path = wkObj.name + "/" + path;
            }
            return path;
        }

        public static void Log(string message)
        {
            Debug.Log($"{ConfigLog.LogPrefix} {message}");
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning($"{ConfigLog.LogPrefix} {message}");
        }

        public static void LogError(string message)
        {
            Debug.LogError($"{ConfigLog.LogPrefix} {message}");
        }

    }
}
