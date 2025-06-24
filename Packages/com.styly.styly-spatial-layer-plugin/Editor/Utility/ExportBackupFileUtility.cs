using System.IO;
using UnityEditor;


namespace Styly.SpatialLayer.Plugin
{
    public class ExportBackupFileUtility
    {
        private static readonly string ManifestFileName = "manifest.json";
        private static readonly string PackageDirectoryPath = "Packages";
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
            File.Copy(Path.Combine(PackageDirectoryPath, ManifestFileName), Path.Combine(destPath, ManifestFileName ));
        }
    }
}