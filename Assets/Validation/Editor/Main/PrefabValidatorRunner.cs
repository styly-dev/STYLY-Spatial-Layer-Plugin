using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Styly.VisionOs.Plugin.Validation
{
    public class PrefabValidatorRunner : MonoBehaviour
    {
        [MenuItem("Assets/STYLY/Validate Prefab (Alpha)", false, 100)]
        private static void ValidateSelectedPrefab()
        {
            GameObject selectedPrefab = Selection.activeObject as GameObject;
            if (selectedPrefab == null)
            {
                Debug.LogError("Prefab is not selected.");
                return;
            }

            // Create a verification management class
            PrefabValidationManager validationManager = new PrefabValidationManager();

            // Added shader verification
            Shader[] allowedShaders = { 
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
                Shader.Find("Universal Render Pipeline/VR/SpacialMapping/Wireframe"),                };
            validationManager.AddValidator(new ShaderValidator(allowedShaders));

            // Added forbidden components
            System.Type[] forbiddenComponents = {
                typeof(Camera),
                typeof(LensFlare), 
                typeof(LineRenderer),
                typeof(Projector),
                typeof(OcclusionArea),
                typeof(OcclusionPortal),
                typeof(Skybox),
                typeof(TilemapRenderer),
                typeof(GraphicRaycaster),
                typeof(Tree),
            };
            validationManager.AddValidator(new ForbiddenComponentsValidator(forbiddenComponents));

            // Perform all verifications and get results
            bool allPassed = validationManager.ValidateAll(selectedPrefab);

            // Output final result to log
            if (allPassed)
            {
                Debug.Log("All verifications have passed!");
            }
            else
            {
                Debug.LogWarning("Some verification failed.");
            }
        }
    }
}
