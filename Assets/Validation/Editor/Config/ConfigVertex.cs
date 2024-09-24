using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public class ConfigVertex : MonoBehaviour
    {
        public static int MaxVertexCount = 100 * 1000; // 100,000 vertices per mesh
        public static int MaxTotalVertexCount = 500 * 1000; // 500,000 vertices in total
    }
}
