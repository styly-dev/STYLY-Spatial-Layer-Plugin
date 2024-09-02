#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class SimpleHttpServerEditor : EditorWindow
{
    private string serverUrl = "";
    private UnityHttpServerManager server;
    private bool serverRunning = false;

    [MenuItem("STYLY/Simple HTTP Server")]
    public static void ShowWindow()
    {
        GetWindow<SimpleHttpServerEditor>("Simple HTTP Server");
    }

    private void OnEnable()
    {
        server = new UnityHttpServerManager();
    }

    private void OnGUI()
    {
        GUILayout.Label("Simple HTTP Server", EditorStyles.boldLabel);

        if (!serverRunning)
        {
            if (GUILayout.Button("Start Server"))
            {
                server.StartServer();
                serverRunning = true;

                serverUrl = $"http://{SimpleHttpServer.GetHostName()}:{server.Port}/";
            }
        }
        else
        {
            if (GUILayout.Button("Stop Server"))
            {
                server.StopServer();
                serverRunning = false;
                serverUrl = "";
            }
        }

        GUILayout.Space(10);

        if (!string.IsNullOrEmpty(serverUrl))
        {
            GUILayout.Label("Server URL:", EditorStyles.boldLabel);
            EditorGUILayout.TextField(serverUrl);

            if (GUILayout.Button("Copy URL"))
            {
                EditorGUIUtility.systemCopyBuffer = serverUrl;
            }

            if (GUILayout.Button("Open in Browser"))
            {
                Application.OpenURL(serverUrl);
            }
        }
    }

    private void OnDisable()
    {
        if (serverRunning)
        {
            server.StopServer();
        }
    }
}
#endif