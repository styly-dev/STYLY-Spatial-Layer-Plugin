using System.IO;
using UnityEditor;


namespace Styly.VisionOs.Plugin
{
    public class ExportPackageUtility
    {
        public static void Export(string sourcePath, string destFilePath)
        {
            var dirPath = Path.GetDirectoryName(destFilePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            
            // Prevent crash on Mac due to conflicts with progress windows, especially when using AssetDatabase.ExportPackage method.
            EditorUtility.ClearProgressBar();

            AssetDatabase.ExportPackage(sourcePath, destFilePath, ExportPackageOptions.IncludeDependencies);
        }
    }
}