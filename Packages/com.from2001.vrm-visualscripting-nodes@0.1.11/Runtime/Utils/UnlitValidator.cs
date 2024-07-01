using System;
using UnityEngine;
using VRMShaders.VRM10.MToon10.Runtime;

// This code is based on Packages/com.vrmc.vrmshaders/VRM10/MToon10/Runtime/MToonValidator.cs
namespace VisualScriptingNodes
{
    /// <summary>
    /// Validator for URP/Unlit
    /// This will be used on VisionOS where MToon shader is not compatible 
    /// </summary>
    public sealed class UnlitValidator
    {
        private readonly Material _material;

        public UnlitValidator(Material material)
        {
            _material = material;
        }

        public void Validate()
        {
            var alphaMode = (MToon10AlphaMode)_material.GetInt(MToon10Prop.AlphaMode);
            var zWriteMode = (MToon10TransparentWithZWriteMode)_material.GetInt(MToon10Prop.TransparentWithZWrite);
            var renderQueueOffset = _material.GetInt(MToon10Prop.RenderQueueOffsetNumber);
            var doubleSidedMode = (MToon10DoubleSidedMode)_material.GetInt(MToon10Prop.DoubleSided);
            SetUnityShaderPassSettings(_material, alphaMode, zWriteMode, renderQueueOffset, doubleSidedMode);
        }

        private static void SetUnityShaderPassSettings(Material material, MToon10AlphaMode alphaMode, MToon10TransparentWithZWriteMode zWriteMode, int renderQueueOffset, MToon10DoubleSidedMode doubleSidedMode)
        {
            // Handle Surface Type (Opaque/Transparent)
            bool isTransparent = alphaMode > 0 || zWriteMode > 0;

            if (isTransparent)
            {
                // Set surface type to Transparent and enable Alpha Clipping
                material.SetFloat("_Surface", 1.0f); // Set to Transparent
                material.SetFloat("_AlphaClip", 1.0f); // Enable Alpha Clipping

                // Set blend modes for transparent materials
                material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0.0f); // Typically, ZWrite is off for transparent materials
            }
            else
            {
                // Set surface type to Opaque and disable Alpha Clipping
                material.SetFloat("_Surface", 0.0f); // Set to Opaque
                material.SetFloat("_AlphaClip", 0.0f); // Disable Alpha Clipping
            }

            switch (doubleSidedMode)
            {
                case MToon10DoubleSidedMode.Off:
                    material.SetFloat("_Cull", 2.0f);
                    break;
                case MToon10DoubleSidedMode.On:
                    material.SetFloat("_Cull", 0.0f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(doubleSidedMode), doubleSidedMode, null);
            }

            // I don't know why this is needed, but it doesn't work without it
            material.shader = Shader.Find("Universal Render Pipeline/Unlit");
        }
    }

}