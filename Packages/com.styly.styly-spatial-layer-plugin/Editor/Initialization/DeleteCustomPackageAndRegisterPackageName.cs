using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using Styly.VisionOs.Plugin;
using System;

namespace Styly
{
    /// <summary>
    /// This class is intended to move the package from Package directry to OpenUPM when the project is downloaded from the release zip.
    /// Just put this file in the Editor folder of the package. No need to specify the package name.
    /// </summary>
    public class DeleteCustomPackageAndRegisterPackageName
    {
        [InitializeOnLoadMethod]
        static void FuncOfDeleteCustomPackageAndRegisterPackageName()
        {
            // If the project is managed with Git, do nothing.
            if (IsProjectManagedWithGit()) { return; }

            // Proceed only when the project seems donwloaded from the release zip.
            var MyPackageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(System.Reflection.MethodInfo.GetCurrentMethod().DeclaringType.Assembly);
            string MyPackageName = MyPackageInfo.name;
            string MyPackageVersion = MyPackageInfo.version;
            string MyPackagePath = MyPackageInfo.resolvedPath;
            string MyPackageSource = MyPackageInfo.source.ToString();

            // If the package is installed with files in Packages and not managed with Git, delete the package folder and register the package using OpenUPM.
            if (MyPackageSource == "Embedded")
            {
                // Delete the package folder (Move the directory to a temporary path for fallback)
                if (!Directory.Exists(MyPackagePath)) { return; }
                string tempPath = MoveDirectoryToTempPath(MyPackagePath);

                // Add the package to the project using OpenUPM
                bool result = PackageManagerUtility.AddUnityPackage(MyPackageName + "@" + MyPackageVersion);

                // Fallback and cleanup
                if (!result)
                {
                    // If the package was not added successfully, restore the directory from the temporary path
                    FileUtil.CopyFileOrDirectory(tempPath, MyPackagePath);
                    Debug.LogError($"{MyPackageName}: Failed to switch the package source to OpenUPM. This will be retried automatically next time.");
                }
                // Cleanup the temporary directory
                if (Directory.Exists(tempPath)) { Directory.Delete(tempPath, true); }
                if (Directory.Exists(Path.GetDirectoryName(tempPath)) && !Directory.EnumerateFileSystemEntries(Path.GetDirectoryName(tempPath)).Any()){Directory.Delete(Path.GetDirectoryName(tempPath), true);}
            }
        }

        /// <summary>
        /// Check if the project is managed with Git
        /// (If .git directory exists at the root of the project or the parent folder of the project directory, return true.)
        /// </summary>
        /// <returns></returns>
        static bool IsProjectManagedWithGit()
        {
            var MyPackageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(System.Reflection.MethodInfo.GetCurrentMethod().DeclaringType.Assembly);

            // Return false if the package is installed with files in Packages folder
            if (MyPackageInfo.source.ToString() != "Embedded") { return false; }

            // Get the root directory of the project
            string MyPackagePath = MyPackageInfo.resolvedPath;
            var ProjectRootDirectry = Directory.GetParent(MyPackagePath).Parent;

            // Check .git directory at the root of the project (or the parent folder of the project directory) 
            if (Directory.Exists(Path.Combine(ProjectRootDirectry.FullName, ".git")) || Directory.Exists(Path.Combine(ProjectRootDirectry.Parent.FullName, ".git"))) { return true; }

            return false;
        }

        /// <summary>
        /// Move the directory to a temporary path.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        static string MoveDirectoryToTempPath(string sourcePath)
        {
            try
            {
                // Create a unique temporary path to avoid name collisions
                var workDirectoryPath = FileUtil.GetUniqueTempPathInProject();
                Directory.CreateDirectory(workDirectoryPath);
                string tempPath = Path.Combine(
                    workDirectoryPath,
                    $"{Path.GetFileName(sourcePath)}_{Guid.NewGuid():N}"
                );
                FileUtil.CopyFileOrDirectory(sourcePath, tempPath);
                Directory.Delete(sourcePath, true);
                return tempPath;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to move directory '{sourcePath}' to a temporary path. Exception: {ex.Message}");
                return null; // Return null to indicate failure
            }
        }


    }
}