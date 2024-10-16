using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public class ComponentsValidator : IPrefabValidator
    {
        private System.Type[] _forbiddenComponentTypes;

        /// <summary>
        /// List of forbidden components
        /// </summary>
        /// <param name="forbiddenComponentTypes"></param>
        public ComponentsValidator(System.Type[] forbiddenComponentTypes)
        {
            _forbiddenComponentTypes = forbiddenComponentTypes;
        }

        public bool Validate(GameObject prefab)
        {
            bool passed = true;

            // Verification of each forbidden component
            foreach (var componentType in _forbiddenComponentTypes)
            {
                Component[] forbiddenComponents = prefab.GetComponentsInChildren(componentType, true);
                if (forbiddenComponents != null)
                {
                    foreach (var forbiddenComponent in forbiddenComponents)
                    {
                        string path = ValidatorUtility.GetGameObjectPath(forbiddenComponent.gameObject);
                        ValidatorUtility.LogWarning($"{componentType.Name} component is used: {path}");
                    }
                    passed = false;
                }
            }

            return passed;
        }
    }
}
