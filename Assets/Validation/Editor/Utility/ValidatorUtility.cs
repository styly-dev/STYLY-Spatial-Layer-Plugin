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
    }
}
