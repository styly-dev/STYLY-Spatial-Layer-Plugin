using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

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

                // Add a scoped registry of the OpenUPM package
                AddScopedRegistryOfOpenUpmPackage(MyPackageName);

                // Add the package to the project
                AddUnityPackage(MyPackageName + "@" + MyPackageVersion);
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
        /// Add a Unity package to the project
        /// </summary>
        /// <param name="packageName">
        /// Example: com.company.packaganame or com.company.packaganame@0.1.1
        /// </param>
        static void AddUnityPackage(string packageName)
        {
            var request = UnityEditor.PackageManager.Client.Add(packageName);
            while (!request.IsCompleted) { }
            if (request.Error != null) { Debug.LogError(request.Error.message); }
        }

        /// <summary>
        /// Add a scoped registry of the OpenUPM package
        /// </summary>
        static void AddScopedRegistryOfOpenUpmPackage(string packageName)
        {
            AddScopedRegistry(new ScopedRegistry
            {
                name = "package.openupm.com",
                url = "https://package.openupm.com",
                scopes = new string[] {
                packageName
            }
            });
        }

        /// <summary>
        /// Add a scoped registry to the manifest.json file only if it doesn't already exist.
        /// </summary>
        static void AddScopedRegistry(ScopedRegistry pScopeRegistry)
        {
            var manifestPath = Path.Combine(Application.dataPath, "..", "Packages/manifest.json");
            var manifestJson = File.ReadAllText(manifestPath);
            var manifest = JsonConvert.DeserializeObject<ManifestJson>(manifestJson);
            var existingRegistry = manifest.scopedRegistries.FirstOrDefault(r => r.name == pScopeRegistry.name);

            if (existingRegistry != null)
            {
                // Check if the scope already exists
                if (!existingRegistry.scopes.Contains(pScopeRegistry.scopes[0]))
                {
                    // Add the new scope to the existing registry
                    var scopesList = existingRegistry.scopes.ToList();
                    scopesList.Add(pScopeRegistry.scopes[0]);
                    existingRegistry.scopes = scopesList.ToArray();
                }
            }
            else
            {
                // Add the new registry
                manifest.scopedRegistries.Add(pScopeRegistry);
            }
            File.WriteAllText(manifestPath, JsonConvert.SerializeObject(manifest, Formatting.Indented));
        }

        class ScopedRegistry
        {
            public string name;
            public string url;
            public string[] scopes;
        }

        class ManifestJson
        {
            public Dictionary<string, string> dependencies = new();
            public List<ScopedRegistry> scopedRegistries = new();
        }
    }
}