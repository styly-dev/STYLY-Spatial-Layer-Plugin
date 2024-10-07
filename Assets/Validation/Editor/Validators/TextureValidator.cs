using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public class TextureValidator : IPrefabValidator
    {
        private int _maxTextureWidth;
        private int _maxTextureHeight;

        public TextureValidator(int maxTextureWidth, int maxTextureHeight)
        {
            _maxTextureWidth = maxTextureWidth;
            _maxTextureHeight = maxTextureHeight;
        }

        public bool Validate(GameObject prefab)
        {
            bool passed = true;
            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);

            StringBuilder sb = new StringBuilder();
            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (material != null)
                    {
                        foreach (string propertyName in material.GetTexturePropertyNames())
                        {
                            Texture texture = material.GetTexture(propertyName);
                            if (texture != null && texture is Texture2D)
                            {
                                Texture2D texture2D = (Texture2D)texture;
                                // テクスチャサイズの検証
                                if (texture2D.width > _maxTextureWidth || texture2D.height > _maxTextureHeight)
                                {
                                    string path = ValidatorUtility.GetGameObjectPath(renderer.gameObject);
                                    sb.Clear();
                                    sb.Append($"{renderer.gameObject.name} has a texture '{texture2D.name}");
                                    sb.Append($" attached that exceeds the maximum size ({_maxTextureWidth}x{_maxTextureHeight}):");
                                    sb.Append($" {texture2D.width}x{texture2D.height}. Attached to object: {path}");
                                    ValidatorUtility.LogError(sb.ToString());
                                    passed = false;
                                }
                            }
                        }
                    }
                }
            }

            return passed;
        }
    }
}
