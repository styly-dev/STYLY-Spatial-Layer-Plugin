using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Styly.VisionOs.Plugin.Validation
{
    public class PrefabValidatorRunner
    {
        [MenuItem("Assets/STYLY/Validate Prefab (Alpha)", priority = 100)]
        private static void ValidateSelectedPrefab()
        {
            GameObject selectedPrefab = Selection.activeObject as GameObject;
            if (selectedPrefab == null)
            {
                ValidatorUtility.LogError("Prefab is not selected.");
                return;
            }

            // Start recursive validation with the selected prefab
            ValidatePrefabRecursively(selectedPrefab);

            // Display execution results in dialog
            if (EditorUtility.DisplayDialog("Validate Prefab (Alpha)", "Prefab validation completed. Please confirm the Unity editor console messages.", "OK"))
            {
                // Do nothing
            }
        }

        // Main recursive validation method
        private static void ValidatePrefabRecursively(GameObject selectedPrefab)
        {
            ValidatorUtility.Log($"Validating prefab: {selectedPrefab.name}");

            // Create a verification management class
            PrefabValidationManager validationManager = new PrefabValidationManager();

            // Add verification items
            validationManager.AddValidator(new ComponentsValidator(ConfigComponent.forbiddenComponents));
            validationManager.AddValidator(new BoundingBoxValidator(ConfigBoundingBox.recommanedSize));
            validationManager.AddValidator(new VertexValidator(ConfigVertex.MaxVertexCount, ConfigVertex.MaxTotalVertexCount));
            validationManager.AddValidator(new GroundValidator());

            //            validationManager.AddValidator(new ShaderValidator(ConfigShaders.allowedShaders));
            //            validationManager.AddValidator(new TextureValidator(ConfigTexture.MaxTextureWidth, ConfigTexture.MaxTextureHeight));


            // Run all validations for the current prefab
            bool allPassed = validationManager.ValidateAll(selectedPrefab);

            // Output final result to log
            ValidatorUtility.Log("Prefab validation completed.");
            if (!allPassed)
            {
                ValidatorUtility.LogWarning("It detected some warnings.");
            }

            // Now check if any components reference other Prefabs
            CheckComponentsForPrefabReferences(selectedPrefab);
        }

        // Method to check each GameObject's components for references to other Prefabs
        // Method to check each GameObject's components for references to other Prefabs
        private static void CheckComponentsForPrefabReferences(GameObject selectedPrefab)
        {
            Transform[] allTransforms = selectedPrefab.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allTransforms)
            {
                GameObject childObject = t.gameObject;

                // Check each component of the GameObject
                Component[] components = childObject.GetComponents<Component>();
                foreach (Component component in components)
                {
                    if (component == null) continue;

                    // Convert the component to a SerializedObject to inspect its properties
                    SerializedObject so = new SerializedObject(component);
                    SerializedProperty sp = so.GetIterator();

                    while (sp.NextVisible(true))
                    {
                        // Check if the property is an object reference and a Prefab
                        if (sp.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            Object referencedObject = sp.objectReferenceValue;

                            if (referencedObject != null && PrefabUtility.IsPartOfPrefabAsset(referencedObject))
                            {
                                GameObject referencedPrefab = referencedObject as GameObject;
                                if (referencedPrefab != null)
                                {
                                    ValidatorUtility.Log($"Found reference to another Prefab: {referencedPrefab.name} in component {component.GetType().Name} on GameObject {childObject.name}");

                                    // Recursively validate the referenced prefab
                                    ValidatePrefabRecursively(referencedPrefab);
                                }
                            }
                        }
                    }
                }
            }
        }

        [MenuItem(@"Assets/STYLY/Validate Prefab (Alpha)", validate = true, priority = 100)]
        static bool ValidateBuildContent()
        {
            if (Application.isPlaying) return false;
            if (Selection.objects.Length != 1) return false;
            var assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
            return IsBuildTargetType(assetPath);
        }
        private static bool IsBuildTargetType(string path)
        {
            return Path.GetExtension(path).ToLower() == ".prefab";
        }
    }
}
