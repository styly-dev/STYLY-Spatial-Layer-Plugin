using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Styly.VisionOs.Plugin.Validation
{
    public class ConfigComponent : MonoBehaviour
    {
        public static System.Type[] forbiddenComponents = {
                typeof(Camera),
                typeof(LensFlare),
                typeof(LineRenderer),
                typeof(Projector),
                typeof(OcclusionArea),
                typeof(OcclusionPortal),
                typeof(Skybox),
                typeof(TilemapRenderer),
                typeof(GraphicRaycaster),
                typeof(Tree),
            };
    }
}
