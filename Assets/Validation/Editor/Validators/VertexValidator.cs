using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    using UnityEngine;

    public class VertexValidator : IPrefabValidator
    {
        private int _maxVertexCountPerMesh;
        private int _maxTotalVertexCount;

        public VertexValidator(int maxVertexCountPerMesh, int maxTotalVertexCount)
        {
            _maxVertexCountPerMesh = maxVertexCountPerMesh;
            _maxTotalVertexCount = maxTotalVertexCount;
        }

        public bool Validate(GameObject prefab)
        {
            bool passed = true;
            int totalVertexCount = 0;
            MeshFilter[] meshFilters = prefab.GetComponentsInChildren<MeshFilter>(true);

            foreach (MeshFilter meshFilter in meshFilters)
            {
                if (meshFilter.sharedMesh != null)
                {
                    int vertexCount = meshFilter.sharedMesh.vertexCount;
                    totalVertexCount += vertexCount;

                    // Check vertex count for individual meshes
                    if (vertexCount > _maxVertexCountPerMesh)
                    {
                        string path = ValidatorUtility.GetGameObjectPath(meshFilter.gameObject);
                        ValidatorUtility.LogWarning($"{meshFilter.gameObject.name} exceeds the maximum vertex count per mesh of {_maxVertexCountPerMesh} ({vertexCount} vertices): {path}");
                        passed = false;
                    }
                }
            }

            // Check total vertex count
            if (totalVertexCount > _maxTotalVertexCount)
            {
                ValidatorUtility.LogWarning($"Total vertex count exceeds the maximum of {_maxTotalVertexCount} vertices: {totalVertexCount} vertices in total.");
                passed = false;
            }

            return passed;
        }
    }
}
