using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public class ShaderValidator : IPrefabValidator
    {
        private Shader[] _allowedShaders;

        public ShaderValidator(Shader[] allowedShaders)
        {
            _allowedShaders = allowedShaders;
        }

        public bool Validate(GameObject prefab)
        {
            bool passed = true;
            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (material != null && System.Array.IndexOf(_allowedShaders, material.shader) == -1)
                    {
                        string path = ValidatorUtility.GetGameObjectPath(renderer.gameObject);
                        ValidatorUtility.LogWarning($"Using unsupported shaders: {material.shader.name}, Game Object: {path}");
                        passed = false;
                    }
                }
            }

            return passed;
        }
    }
}
