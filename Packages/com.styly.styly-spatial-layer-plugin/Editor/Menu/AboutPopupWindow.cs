using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Styly.VisionOs.Plugin
{
    public class AboutPopupWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("Assets/STYLY/About", false, 90009)]
        public static void ShowAboutStyly()
        {
            var wnd = GetWindow(typeof(AboutPopupWindow), true, "About STYLY");
            wnd.titleContent = new GUIContent("About STYLY");
            wnd.maxSize = new Vector2(600, 200);
            wnd.minSize = new Vector2(600, 200);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
        
            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            var packageVersion = PackageManagerUtility.GetPackageVersion(Config.PackageName);
            var versionLabel = root.Q<Label>("PluginVersionLabel");
            versionLabel.text = packageVersion;

            var unityVersionLabel = root.Q<Label>("UnityVersionLabel");
            unityVersionLabel.text = Application.unityVersion;
        }
    }
}
