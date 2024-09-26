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

            // Add verification items
//            validationManager.AddValidator(new ShaderValidator(ConfigShaders.allowedShaders));
            validationManager.AddValidator(new ComponentsValidator(ConfigComponent.forbiddenComponents));
            validationManager.AddValidator(new BoundingBoxValidator(ConfigBoundingBox.recommanedSize));
            validationManager.AddValidator(new VertexValidator(ConfigVertex.MaxVertexCount, ConfigVertex.MaxTotalVertexCount));
//            validationManager.AddValidator(new TextureValidator(ConfigTexture.MaxTextureWidth, ConfigTexture.MaxTextureHeight));
            validationManager.AddValidator(new GroundValidator());

            // Perform all verifications and get results
            bool allPassed = validationManager.ValidateAll(selectedPrefab);

            // Output final result to log
            Debug.Log("Prefab validation completed.");
            if (!allPassed)
            {
                Debug.LogWarning("It detected some warnings.");
            }

            // Display execution results in dialog
            if (EditorUtility.DisplayDialog("Validate Prefab (Alpha)", "Prefab validation completed. Please confirm the Unity editor console messages.", "OK"))
            {
                // Do nothing
            }
        }
    }
}
