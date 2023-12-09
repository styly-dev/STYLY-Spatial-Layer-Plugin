using System.IO;
using UnityEditor;


namespace Styly.VisionOs.Plugin
{
    public class ExportBackupFileUtility
    {
        private static readonly string ManifestFilePath = "Packages/manifest.json";
        private static readonly string BackupUnityPackageFileName = "backup.unitypackage";
        public static void Export(string sourcePath, string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            
            // Prevent crash on Mac due to conflicts with progress windows, especially when using AssetDatabase.ExportPackage method.
            EditorUtility.ClearProgressBar();

            AssetDatabase.ExportPackage(sourcePath, Path.Combine(destPath, BackupUnityPackageFileName) , ExportPackageOptions.IncludeDependencies);
            File.Copy(ManifestFilePath, Path.Combine(destPath, "manifest.json"));
        }
    }
}