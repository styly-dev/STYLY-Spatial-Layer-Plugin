using UnityEngine;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using System.Collections;
using GLTFast;
using UnityEngine.Networking;
using System;
using System.IO;
using STYLY.Http;
using STYLY.Http.Service;
using UnityGLTF;

namespace GltfastVisualScriptingNodes
{
    [UnitShortTitle("Load glTF/glb")]
    [UnitTitle("Load glTF/glb")]
    [UnitCategory("Gltf")]
    [UnitSubtitle("Load glTF/glb with URL")]
    public class LoadGltf : Unit
    {
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;

        [DoNotSerialize]
        public ValueInput glTF_URL;

        [DoNotSerialize]
        public ValueInput TargetGameobject;

        [DoNotSerialize]
        public ValueInput NormalizeScale;
        
        
        [DoNotSerialize]
        public ValueInput rotateAmount;

        [DoNotSerialize]
        public ValueOutput result;

        private GameObject resultValue;
        protected override void Definition()
        {
            inputTrigger = ControlInputCoroutine("inputTrigger", Enter);
            outputTrigger = ControlOutput("outputTrigger");

            glTF_URL = ValueInput<string>("glTF/glb URL", "");
            TargetGameobject = ValueInput<GameObject>("Target Game Object", null);
            // Added rotateAmount input
            rotateAmount = ValueInput<Vector3>("rotateAmount", Vector3.zero);  
            NormalizeScale = ValueInput<bool>("Normalize Scale", true);
            result = ValueOutput<GameObject>("Game Object", (flow) => resultValue);
        }

        private IEnumerator Enter(Flow flow)
        {
            string url = flow.GetValue<string>(glTF_URL);
            GameObject target = flow.GetValue<GameObject>(TargetGameobject);
            Vector3 rotation = flow.GetValue<Vector3>(rotateAmount);  // Get the rotateAmount value
            bool adjustScale = flow.GetValue<bool>(NormalizeScale);
            GameObject gltfInstance = null;

            UniTask.Create(async () =>
            {
                gltfInstance = await LoadGltfWithURL(url, target, adjustScale);
            }).Forget();

            yield return new WaitUntil(() => gltfInstance != null);

            // Apply rotation
            if (target != null)
            {
                target.transform.eulerAngles = rotation;
            }
            else if (gltfInstance != null)
            {
                gltfInstance.transform.eulerAngles = rotation;
            }

            resultValue = gltfInstance;
            yield return outputTrigger;
        }



        /// <summary>
        /// Load glTF/glb with URL using UnityGLTF
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="target"></param>
        /// <param name="normalizescale"></param>
        /// <returns></returns>
        private async UniTask<GameObject> LoadGltfWithURL(string URL, GameObject target = null, bool normalizescale = true)
        {
            // Create glTF GameObject
            GameObject glTF = new("glTF");
            var UnityGltfScript = glTF.AddComponent<GLTFComponent>();

            // Set glTF loading parameters
            UnityGltfScript.GLTFUri = URL;
            UnityGltfScript.Multithreaded = true;
            UnityGltfScript.UseStream = true;
            UnityGltfScript.AppendStreamingAssets = false;
            UnityGltfScript.PlayAnimationOnLoad = true;
            UnityGltfScript.HideSceneObjDuringLoad = false;
            UnityGltfScript.Factory = null;

            // Load glTF/glb
            await UnityGltfScript.Load();

            // Adjust scale to 1 unit size
            if (normalizescale) Utils.FitToUnitSize(glTF);

            // Set glTF location to Target Game Object
            if (target != null)
            {
                glTF.transform.SetParent(target.transform);
                glTF.transform.localPosition = Vector3.zero;
                glTF.transform.localRotation = Quaternion.identity;
                glTF.transform.localScale = Vector3.one;
            }

            return glTF;
        }

        // This method is not used now.
        private async UniTask<GameObject> LoadGltfWithURL_glTFast(string URL, GameObject target = null, bool normalizescale = true)
        {
            GameObject glTF = new("glTF"); // This is the parent object of the glTF instance. Always 1 unit size.
            GameObject gltfInstance = new("glTFast");
            glTF.SetActive(false);
            gltfInstance.transform.SetParent(glTF.transform);
            byte[] GlbBytes = null;
            try
            {
                HttpResponse httpResponse = await Http.Get(URL)
                                .UseCache(CacheType.UseCacheAlways)
                                .OnError(response => Debug.Log(response.StatusCode))
                                .SendAsync();
                if (httpResponse.IsSuccessful)
                {
                    GlbBytes = httpResponse.Bytes;

                    // Use glTfast materials since PBRGraphMaterialGenerator is broken.
                    // GLTFast.Materials.IMaterialGenerator materialGenerator = Utils.IsVisionOS() ? new PBRGraphMaterialGenerator(new MemoryStream(GlbBytes)) : null;
                    GLTFast.Materials.IMaterialGenerator materialGenerator = null;

                    var gltfImport = new GltfImport(null, null, materialGenerator, null);
                    var instantiator = new GameObjectInstantiator(gltfImport, gltfInstance.transform);

                    bool success_load = await gltfImport.LoadGltfBinary(
                        GlbBytes,
                        new Uri(URL)
                        );
                    if (success_load)
                    {
                        bool success_instantiniate = await gltfImport.InstantiateMainSceneAsync(instantiator);
                        if (success_instantiniate)
                        {
                            // Adjust scale to 1 unit size
                            if (normalizescale) Utils.FitToUnitSize(gltfInstance);

                            // Set glTF location to Target Game Object
                            if (target != null)
                            {
                                glTF.transform.SetParent(target.transform);
                                glTF.transform.localPosition = Vector3.zero;
                                glTF.transform.localRotation = Quaternion.identity;
                                glTF.transform.localScale = Vector3.one;
                            }

                            // Play animation
                            var legacyAnimation = instantiator.SceneInstance.LegacyAnimation;
                            if (legacyAnimation != null) legacyAnimation.Play();
                        }
                        else
                        {
                            Debug.LogError("Failed to instantiate glTF/glb from URL: " + URL);
                            UnityEngine.Object.Destroy(glTF);
                            return null;
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to LoadGltfBinary glTF/glb from URL: " + URL);
                        UnityEngine.Object.Destroy(glTF);
                        return null;
                    }
                }
                else
                {
                    Debug.LogError("Failed to download glTF/glb from URL: " + URL);
                    UnityEngine.Object.Destroy(glTF);
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("Failed to download glTF/glb from URL: " + URL);
                UnityEngine.Object.Destroy(glTF);
                return null;
            }
            glTF.SetActive(true);
            return glTF;
        }
    }
}