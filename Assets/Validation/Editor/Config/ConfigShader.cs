using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public class ConfigShaders
    {
        // Added shader verification
        public static Shader[] allowedShaders = {
                Shader.Find("Universal Render Pipeline/Lit"),
                Shader.Find("Universal Render Pipeline/Unlit"),
                Shader.Find("Universal Render Pipeline/Baked Lit"),
                Shader.Find("Universal Render Pipeline/Complex Lit"),
                Shader.Find("Universal Render Pipeline/Simple Lit"),
                Shader.Find("Universal Render Pipeline/Unlit"),

                // 2D Shader
                Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"),
                Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"),

                // Autodesk Interactive Shader
                Shader.Find("Universal Render Pipeline/Autodesk Interactive/AutodeskInteractive"),
                Shader.Find("Universal Render Pipeline/Autodesk Interactive/AutodeskInteractiveTransparent"),

                // Particles Shader
                Shader.Find("Universal Render Pipeline/Particles/Lit"),
                Shader.Find("Universal Render Pipeline/Particles/Simple Lit"),
                Shader.Find("Universal Render Pipeline/Particles/Unlit"),

                // VR Shader
                Shader.Find("Universal Render Pipeline/VR/SpacialMapping/Occlusion"),
                Shader.Find("Universal Render Pipeline/VR/SpacialMapping/Wireframe"),

                // Text Mesh Pro Shader
                Shader.Find("TextMeshPro/Mobile/Distance Field"),
            };
    }
}
