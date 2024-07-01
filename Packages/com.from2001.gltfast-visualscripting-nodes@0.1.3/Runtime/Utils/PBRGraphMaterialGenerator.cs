using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Material = UnityEngine.Material;

namespace GltfastVisualScriptingNodes
{
    public class PBRGraphMaterialGenerator : GLTFast.Materials.MaterialGenerator
    {
        private Stream _glbStream;
        private Dictionary<string, Material> _materialsByUnityGLTF = new();

        /// <summary>
        /// Constructor to set glb stream and get all materials with UnityGLTF
        /// </summary>
        /// <param name="stream"></param>
        public PBRGraphMaterialGenerator(Stream stream){
            _glbStream = stream;
            var task = GetMaterialsWithUnityGLTFAsync();
            task.Wait();
            _materialsByUnityGLTF = task.Result;
        }

        protected override Material GenerateDefaultMaterial(bool pointsSupport = false)
        {
            return new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }

        public override Material GenerateMaterial(
            GLTFast.Schema.MaterialBase gltfMaterial,
            GLTFast.IGltfReadable gltf,
            bool pointsSupport = false
            )
        {
            Material material = _materialsByUnityGLTF[gltfMaterial.name];
            return material;
        }

        /// <summary>
        /// Get all materials from glb strem with UnityGLTF
        /// </summary>
        /// <returns></returns>
        private async Task<Dictionary<string, Material> > GetMaterialsWithUnityGLTFAsync()
        {
            Dictionary<string, Material> MaterialsDic = new();
            GLTF.GLTFParser.ParseJson(_glbStream, out GLTF.Schema.GLTFRoot gLTFRoot);
            _glbStream.Position = 0;
            var loaderUnityGLTF = new UnityGLTF.GLTFSceneImporter(gLTFRoot, _glbStream, new UnityGLTF.ImportOptions());
            for (int i = 0; i < gLTFRoot.Materials.Count; i++)
            {
                Material m = await loaderUnityGLTF.LoadMaterialAsync(i);
                MaterialsDic.Add(m.name, m);
            }
            return MaterialsDic;
        }



    }

}