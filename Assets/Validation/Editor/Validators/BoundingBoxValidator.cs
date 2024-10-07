using System;
using UnityEditor;
using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public class BoundingBoxValidator : IPrefabValidator
    {
        private Vector3 _recommanedSize;

        public BoundingBoxValidator(Vector3 recommendedSize)
        {
            _recommanedSize = recommendedSize;
        }

        public bool Validate(GameObject prefab)
        {
            bool passed = true;

            Bounds prefabBounds = CalculatePrefabBounds(prefab);

            if (prefabBounds.size == Vector3.zero)
            {
                string path = ValidatorUtility.GetGameObjectPath(prefab.gameObject);
                ValidatorUtility.LogWarning($"Prefab '{path}' has no renderers to calculate bounds.");
                return false;
            }

            // Check if the bounding box size exceeds the recommended size
            if (IsBoundsTooLarge(prefabBounds.size, _recommanedSize))
            {
                LogBoundsWarning(prefab, prefabBounds.size);
                passed = false;
            }
            else
            {
                LogBoundsInfo(prefab, prefabBounds.size);
            }

            return passed;
        }
        private static Bounds CalculatePrefabBounds(GameObject prefab)
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            bool hasBounds = false;

            // Get all Renderer components in the prefab, including inactive ones
            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            // To calculate bounds in local space, uncomment the following lines:
            /*
            foreach (Renderer renderer in renderers)
            {
                if (!hasBounds)
                {
                    bounds = renderer.localBounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.localBounds);
                }
            }
            */

            return bounds;
        }
        private bool IsBoundsTooLarge(Vector3 size, Vector3 recommanedSize)
        {
            return size.x > recommanedSize.x || size.y > recommanedSize.y || size.z > recommanedSize.z;
        }

        private void LogBoundsWarning(GameObject prefab, Vector3 size)
        {
            string path = ValidatorUtility.GetGameObjectPath(prefab.gameObject);
            ValidatorUtility.LogWarning($"Prefab '{path}' has large bounds: {size}");
        }

        private void LogBoundsInfo(GameObject prefab, Vector3 size)
        {
            string path = ValidatorUtility.GetGameObjectPath(prefab.gameObject);
            ValidatorUtility.Log($"Prefab '{path}' has bounds: {size}");
        }
    }
}
