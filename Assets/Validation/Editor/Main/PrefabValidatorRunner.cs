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
            validationManager.AddValidator(new ShaderValidator(ConfigShaders.allowedShaders));
            validationManager.AddValidator(new ComponentsValidator(ConfigComponent.forbiddenComponents));
            validationManager.AddValidator(new BoundingBoxValidator(ConfigBoundingBox.recommanedSize));

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
