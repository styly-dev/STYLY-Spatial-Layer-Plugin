using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using Styly.VisionOs.Plugin;

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

            if (MyPackageSource == "Embedded")
            {
                // Delete the package folder
                if (Directory.Exists(MyPackagePath)) { Directory.Delete(MyPackagePath, true); }

                // Add the package to the project
                PackageManagerUtility.AddUnityPackage(MyPackageName + "@" + MyPackageVersion);
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


    }
}