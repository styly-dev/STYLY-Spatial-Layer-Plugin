using System.Collections.Generic;
using UnityEngine;
using VRMShaders;
using UniVRM10;
using UniGLTF;
using System;

// This code is based on Packages/com.vrmc.vrm/Runtime/IO/Material/URP/Import/UrpVrm10MaterialDescriptorGenerator.cs:
namespace VisualScriptingNodes
{
    /// <summary>
    /// VRM MaterialDescriptorGenerator for URP Unlit
    /// This will be used on VisionOS where MToon shader is not compatible 
    /// </summary>
    public sealed class UrpUnlitMaterialDescriptorGenerator : IMaterialDescriptorGenerator
    {
        public MaterialDescriptor Get(GltfData data, int i)
        {
            // mtoon
            if (UrpVrm10MToonMaterialImporter.TryCreateParam(data, i, out var matDesc))
            {
                // Change MaterialDescriptor based on MaterialDescriptor for MToon
                var matDesc_replaced_with_Unlit = new MaterialDescriptor(
                    matDesc.Name,
                    Shader.Find("Universal Render Pipeline/Unlit"),
                    null,
                    replaceTextureSlots(matDesc.TextureSlots),
                    matDesc.FloatValues,
                    replaceColors(matDesc.Colors),
                    matDesc.Vectors,
                    new Action<Material>[]
                    {
                        material =>
                        {
                            new UnlitValidator(material).Validate();
                        }
                    });

                return matDesc_replaced_with_Unlit;
            }
            // unlit
            if (BuiltInGltfUnlitMaterialImporter.TryCreateParam(data, i, out matDesc)) return matDesc;
            // pbr
            if (UrpGltfPbrMaterialImporter.TryCreateParam(data, i, out matDesc)) return matDesc;

            // fallback
            Debug.LogWarning($"material: {i} out of range. fallback");
            return new MaterialDescriptor(
                GltfMaterialImportUtils.ImportMaterialName(i, null),
                UrpGltfPbrMaterialImporter.Shader,
                null,
                new Dictionary<string, TextureDescriptor>(),
                new Dictionary<string, float>(),
                new Dictionary<string, Color>(),
                new Dictionary<string, Vector4>(),
                new Action<Material>[] { });
        }

        public MaterialDescriptor GetGltfDefault()
        {
            return UrpGltfDefaultMaterialImporter.CreateParam();
        }

        /// <summary>
        /// Replace "_MainTex" with "_BaseMap". Discard other texture slots such as "_BumpMap", "_EmissionMap", "_ShadeTex", "_MatcapTex" etc.
        /// </summary>
        /// <param name="TextureSlots"></param>
        /// <returns></returns>
        IReadOnlyDictionary<string, TextureDescriptor> replaceTextureSlots(IReadOnlyDictionary<string, TextureDescriptor> TextureSlots)
        {
            var UnlitTextureSlots = new Dictionary<string, TextureDescriptor>();
            foreach (var textureSlot in TextureSlots)
            {
                if (textureSlot.Key == "_MainTex")
                {
                    UnlitTextureSlots.Add("_BaseMap", textureSlot.Value);
                }
            }
            return UnlitTextureSlots;
        }

        /// <summary>
        /// Replace "_Color" with "_BaseColor". Discard other colors such as "_EmissionColor", "_ShadeColor", "_RimColor" etc.
        /// </summary>
        /// <param name="Colors"></param>
        /// <returns></returns>
        IReadOnlyDictionary<string, Color> replaceColors(IReadOnlyDictionary<string, Color> Colors)
        {
            var UnlitColors = new Dictionary<string, Color>();
            foreach (var color in Colors)
            {
                if (color.Key == "_Color")
                {
                    UnlitColors.Add("_BaseColor", color.Value);
                }
            }
            return UnlitColors;
        }
    }
}