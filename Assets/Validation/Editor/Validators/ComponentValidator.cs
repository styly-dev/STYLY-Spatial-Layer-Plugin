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
                Component forbiddenComponent = prefab.GetComponentInChildren(componentType, true);
                if (forbiddenComponent != null)
                {
                    string path = ValidatorUtility.GetGameObjectPath(forbiddenComponent.gameObject);
                    Debug.LogWarning($"{componentType.Name} component is used: {path}");
                    passed = false;
                }
            }

            return passed;
        }
    }
}
