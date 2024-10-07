using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public class GroundValidator : IPrefabValidator
    {

        public bool Validate(GameObject prefab)
        {
            bool passed = true;
            Transform[] transforms = prefab.GetComponentsInChildren<Transform>(true);

            foreach (Transform transform in transforms)
            {
                // Check if the world position is below (*,0,*)
                if (transform.position.y < 0)
                {
                    string path = ValidatorUtility.GetGameObjectPath(transform.gameObject);
                    ValidatorUtility.LogWarning($"{transform.gameObject.name} has a position below (*, 0, *) in world coordinates: {transform.position}. Attached to object: {path}");
                    passed = false;
                }
            }

            return passed;
        }

    }
}
