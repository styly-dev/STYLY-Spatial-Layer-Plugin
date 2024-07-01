using UnityEngine;
using Unity.VisualScripting;
using UniVRM10;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using VisualScriptingNodes;
using STYLY.Http;
using STYLY.Http.Service;
using Segur.PolySpatialEnvironmentDiffuseShader.Runtime;

namespace VrmVisualScriptingNodes
{
    [UnitShortTitle("Load VRM")]
    [UnitTitle("Load VRM")]
    [UnitCategory("VRM")]
    [UnitSubtitle("Load VRM with URL")]
    public class LoadVRM : Unit
    {
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;

        [DoNotSerialize]
        public ValueInput VrmURL;

        [DoNotSerialize]
        public ValueInput TargetGameobject;

        [DoNotSerialize]
        public ValueOutput result;
        
        [DoNotSerialize]
        public ValueInput rotateOnYAxis;

        private GameObject resultValue;
        protected override void Definition()
        {
            inputTrigger = ControlInputCoroutine("inputTrigger", Enter);
            outputTrigger = ControlOutput("outputTrigger");

            VrmURL = ValueInput<string>("VRM URL", "");
            TargetGameobject = ValueInput<GameObject>("Target Game Object", null);
            result = ValueOutput<GameObject>("Game Object", (flow) => resultValue);
            rotateOnYAxis = ValueInput<bool>("RotateOnYAxis", false); // default to false

        }

        private IEnumerator Enter(Flow flow)
        {
            string url = flow.GetValue<string>(VrmURL);
            Vrm10Instance vrmInstance = null;

            // Load VRM
            UniTask.Create(async () => { vrmInstance = await LoadVrm(url); }).Forget();
            yield return new WaitUntil(() => vrmInstance);

            // Set VRM location to Target Game Object
            var Target = flow.GetValue<GameObject>(TargetGameobject);
            var shouldRotate = flow.GetValue<bool>(rotateOnYAxis);
            if (Target != null)
            {
                vrmInstance.transform.SetParent(Target.transform);
                vrmInstance.transform.localPosition = Vector3.zero;
                vrmInstance.transform.localRotation = Quaternion.identity;
                vrmInstance.transform.localScale = Vector3.one;
            }
            // Rotate the GameObject if the boolean is true
            if (vrmInstance.gameObject != null && shouldRotate)
            {
                vrmInstance.gameObject.transform.Rotate(0, 180, 0);
            }
            
            resultValue = vrmInstance.gameObject;
            yield return outputTrigger;
        }

        /// <summary>
        /// Load VRM from URL
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        private async UniTask<Vrm10Instance> LoadVrm(string URL)
        {
            byte[] VrmBytes = null;

            HttpResponse httpResponse = await Http.Get(URL)
                .UseCache(CacheType.UseCacheAlways)
                .OnError(response => Debug.Log(response.StatusCode))
                .SendAsync();
            if(httpResponse.IsSuccessful)
            {
                VrmBytes = httpResponse.Bytes;
            }
            
            if (!Utils.IsVisionOS())
            {
                Vrm10Instance vrmInstance = await Vrm10.LoadBytesAsync(
                    VrmBytes,
                    canLoadVrm0X: true,
                    materialGenerator: GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset
                        ? new UrpVrm10MaterialDescriptorGenerator() : null
                );
                return vrmInstance;
            }
            else
            {   
                // Set MaterialDescriptorGenerator for VisionOS

                // Unlit Material 
                // var MaterialDescriptorGenerator_visionOS = new UrpUnlitMaterialDescriptorGenerator();

                // Environment Diffuse Shader Material By segurvita
                var MaterialDescriptorGenerator_visionOS = new EnvironmentDiffuseMaterialDescriptorGenerator();

                Vrm10Instance vrmInstance = await Vrm10.LoadBytesAsync(
                    VrmBytes,
                    canLoadVrm0X: true,
                    materialGenerator: GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset
                        ? MaterialDescriptorGenerator_visionOS : null
                );
                return vrmInstance;
            }

        }
    }
}