using System;
using UnityEditor;
using UnityEngine;

namespace Styly.VisionOs.Plugin
{
    public class OpenWebUI
    {
        [MenuItem(@"Assets/STYLY/Open Web UI", false, 10001)]
        private static void Open()
        {
            var uri = new Uri(Config.UploadPage);
            Application.OpenURL(uri.AbsoluteUri);
        }
    }
}
