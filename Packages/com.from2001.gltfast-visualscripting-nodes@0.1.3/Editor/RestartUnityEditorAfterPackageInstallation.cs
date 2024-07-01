using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;

/// <summary>
/// Restart Unity Editor after package installation
/// </summary>
class RestartUnityEditorAfterPackageInstallation
{
    [InitializeOnLoadMethod]
    static void CheckNeedRestart()
    {
        // Get the infomation of the package of this script 
        var MyPackageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(System.Reflection.MethodInfo.GetCurrentMethod().DeclaringType.Assembly);
        string MyPackageName = MyPackageInfo.name;
        string MyPackageVersion = GetPackageVersion(MyPackageName);

        if (EditorUserSettings.GetConfigValue("VersionOf_" + MyPackageName) != MyPackageVersion)
        {
            EditorUserSettings.SetConfigValue("VersionOf_" + MyPackageName, MyPackageVersion);
            if (EditorUtility.DisplayDialog("Restart Unity",
                "You need to restart Unity to apply the new changes. Restart now?",
                "Restart", "Later"))
            {
                // Restart Unity Editor
                EditorApplication.OpenProject(System.Environment.CurrentDirectory);
            }
            else
            {
                // Inform the user to restart Unity manually
                EditorUtility.DisplayDialog("Manual Restart Required",
                    "Please close and reopen Unity to complete the update.",
                    "OK");
            }
        }
    }

    /// <summary>
    /// Get the version of the package
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    private static string GetPackageVersion(string packageName)
    {
        var request = Client.List(true, true);
        while (!request.IsCompleted) { }
        return request.Result.FirstOrDefault(package => package.name == packageName)?.version;
    }
}