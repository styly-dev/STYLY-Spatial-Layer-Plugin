using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public class ForbiddenComponentsValidator : IPrefabValidator
    {
        private System.Type[] forbiddenComponentTypes;

        /// <summary>
        /// List of forbidden components
        /// </summary>
        /// <param name="forbiddenComponentTypes"></param>
        public ForbiddenComponentsValidator(System.Type[] forbiddenComponentTypes)
        {
            this.forbiddenComponentTypes = forbiddenComponentTypes;
        }

        public bool Validate(GameObject prefab)
        {
            bool passed = true;

            // Verification of each forbidden component
            foreach (var componentType in forbiddenComponentTypes)
            {
                Component forbiddenComponent = prefab.GetComponentInChildren(componentType, true);
                if (forbiddenComponent != null)
                {
                    string path = GetGameObjectPath(forbiddenComponent.gameObject);
                    Debug.LogWarning($"{componentType.Name} component is used: {path}");
                    passed = false;
                }
            }

            return passed;
        }

        private string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "/" + path;
            }
            return path;
        }
    }
}
