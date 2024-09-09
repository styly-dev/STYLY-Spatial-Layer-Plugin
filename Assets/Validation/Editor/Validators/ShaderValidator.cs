using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public class ShaderValidator : IPrefabValidator
    {
        private Shader[] allowedShaders;

        public ShaderValidator(Shader[] allowedShaders)
        {
            this.allowedShaders = allowedShaders;
        }

        public bool Validate(GameObject prefab)
        {
            bool passed = true;
            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (material != null && System.Array.IndexOf(allowedShaders, material.shader) == -1)
                    {
                        string path = GetGameObjectPath(renderer.gameObject);
                        Debug.LogWarning($"Using unauthorized shaders: {material.shader.name}, Game Object: {path}");
                        passed = false;
                    }
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
