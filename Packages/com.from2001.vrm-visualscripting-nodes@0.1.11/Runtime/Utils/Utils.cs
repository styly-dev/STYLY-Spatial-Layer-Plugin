using UnityEngine;
using Unity.VisualScripting;
using System.Collections.Generic;

namespace VisualScriptingNodes
{
    public class Utils : MonoBehaviour
    {
        /// <summary>
        /// Return true if current OS is VisionOS.
        /// </summary>
        /// <returns></returns>
        public static bool IsVisionOS()
        {
            //When operatingSystem is "visionOS", return true
            if (SystemInfo.operatingSystem.Contains("visionOS")) return true;
            if (Application.platform == RuntimePlatform.VisionOS) return true;
#if UNITY_VISIONOS
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// Return all GameObjects in scene.
        /// </summary>
        /// <returns></returns>
        public static List<GameObject> GetAllObjectsInScene()
        {
            List<GameObject> objectsInScene = new List<GameObject>();
            foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                objectsInScene.Add(obj);
                GetChildObjects(obj, ref objectsInScene);
            }
            return objectsInScene;

            static void GetChildObjects(GameObject obj, ref List<GameObject> objectsInScene)
            {
                foreach (Transform child in obj.transform)
                {
                    objectsInScene.Add(child.gameObject);
                    GetChildObjects(child.gameObject, ref objectsInScene);
                }
            }
        }

        /// <summary>
        /// Change all shaders of GameObject to new one.
        /// This method is intended to be used for VRM and glTF models on VisionOS.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="newShader">"Universal Render Pipeline/Unlit", "Universal Render Pipeline/Lit", etc.</param>
        /// <param name="texturePropertyName_old">Set "_MainTex" for Built-in shader</param>
        /// <param name="texturePropertyName_new">Set "_BaseMap" for URP Shader</param>
        public static void ChangeShadersWithTexture(GameObject targetObject, string newShader = "Universal Render Pipeline/Unlit", string texturePropertyName_old = "_MainTex", string texturePropertyName_new = "_BaseMap")
        {
            SkinnedMeshRenderer[] skinedMeshrenderers = targetObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in skinedMeshrenderers) foreach (Material mat in renderer.materials) ChangeShader(mat, newShader);

            MeshRenderer[] meshrenderers = targetObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in meshrenderers) foreach (Material mat in renderer.materials) ChangeShader(mat, newShader);

            void ChangeShader(Material mat, string shaderName)
            {
                if (mat.HasProperty(texturePropertyName_old))
                {
                    Texture texture = mat.GetTexture(texturePropertyName_old);
                    mat.shader = Shader.Find(shaderName);
                    mat.SetTexture(texturePropertyName_new, texture);
                }
                else
                {
                    mat.shader = Shader.Find(newShader);
                }
            }
        }
    }
}