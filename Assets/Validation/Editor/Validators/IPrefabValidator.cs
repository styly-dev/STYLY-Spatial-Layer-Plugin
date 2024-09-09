using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public interface IPrefabValidator
    {
        bool Validate(GameObject prefab);
    }
}