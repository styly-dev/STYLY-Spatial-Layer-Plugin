using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public class ConfigShaders : MonoBehaviour
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
        //        Shader.Find("Universal Render Pipeline/2D/Sprite-Mask"), // not supported in PolySpacial

                // Autodesk Interactive Shader
                Shader.Find("Universal Render Pipeline/Autodesk Interactive/AutodeskInteractive"),
        //        Shader.Find("Universal Render Pipeline/Autodesk Interactive/AutodeskInteractiveMasked"),// not supported in PolySpacial
                Shader.Find("Universal Render Pipeline/Autodesk Interactive/AutodeskInteractiveTransparent"),

                // SpeedTree Shader  // not supported in PolySpacial
        //        Shader.Find("Universal Render Pipeline/Nature/SpeedTree7"),
        //        Shader.Find("Universal Render Pipeline/Nature/SpeedTree7 Billboard"),
        //        Shader.Find("Universal Render Pipeline/Nature/SpeedTree8"),
        //        Shader.Find("Universal Render Pipeline/Nature/SpeedTree8_PBRLit"),

                // Particles Shader
                Shader.Find("Universal Render Pipeline/Particles/Lit"),
                Shader.Find("Universal Render Pipeline/Particles/Simple Lit"),
                Shader.Find("Universal Render Pipeline/Particles/Unlit"),

                // Terrain Shader // not supported in PolySpacial
        //        Shader.Find("Universal Render Pipeline/Terrain/Lit"),

                // VR Shader
                Shader.Find("Universal Render Pipeline/VR/SpacialMapping/Occlusion"),
                Shader.Find("Universal Render Pipeline/VR/SpacialMapping/Wireframe"),

                // Text Mesh Pro Shader
                Shader.Find("TextMeshPro/Mobile/Distance Field"),
            };
    }
}
