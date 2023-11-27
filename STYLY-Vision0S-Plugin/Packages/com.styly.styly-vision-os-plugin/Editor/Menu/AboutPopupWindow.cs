using Styly.VisionOs.Plugin;
using UnityEditor;
using UnityEngine;

public class AboutPopupWindow : EditorWindow
{
    private Texture2D logo;
    private GUIStyle linkStyle;

    [MenuItem("Assets/STYLY/About", false, 90009)]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow(typeof(AboutPopupWindow), true, "About STYLY");
        window.maxSize = new Vector2(600, 200);
        window.minSize = new Vector2(600, 200);
    }

    private void OnEnable()
    {
        // Link style
        linkStyle = new GUIStyle();
        linkStyle.fontStyle = FontStyle.Italic;
        linkStyle.hover.textColor = new Color(0.8f, 0.8f, 1f);
        linkStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 1f) : Color.blue;

        // Load STYLY logo
        string logoPath = $"Packages/{Config.PackageName}/Editor/UI/STYLY.png"; // ロゴの相対パス
        logo = AssetDatabase.LoadAssetAtPath<Texture2D>(logoPath);
    }


    void OnGUI()
    {
        if (logo != null)
        {
            // ロゴの表示
            GUILayout.Label(logo, GUILayout.Width(1143 / 10), GUILayout.Height(417 / 10));
        }
        else
        {
            EditorGUILayout.LabelField($"ロゴが読み込めません。パッケージ名が{Config.PackageName}から変更されましたか？");
        }

        EditorGUILayout.LabelField("STYLY Vision OS Plugin for Unity");
        EditorGUILayout.LabelField("Unity Version: " + Application.unityVersion);
        EditorGUILayout.LabelField("Plugin Version: " + Styly.VisionOs.Plugin.PackageManagerUtility.Instance.GetPackageVersion(Config.PackageName));

        // サイトへのリンク
        // ハイパーリンクのようなテキストの表示
        if (GUILayout.Button("Visit Our Website", linkStyle))
        {
            Application.OpenURL("https://styly.cc/");
        }

        // マウスオーバー時のカーソル変更
        Rect buttonRect = GUILayoutUtility.GetLastRect();
        if (buttonRect.Contains(Event.current.mousePosition))
        {
            EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);
        }
    }
}
