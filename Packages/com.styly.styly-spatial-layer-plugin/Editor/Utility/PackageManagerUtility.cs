using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager;
using UnityEngine;
using Styly;
using Newtonsoft.Json;
using System.Linq;

namespace Styly.VisionOs.Plugin
{
    public class PackageManagerUtility
    {
        /// <summary>
        /// Check if the project is managed with Git
        /// (If .git directory exists at the root of the project or the parent folder of the project directory, return true.)
        /// </summary>
        /// <returns></returns>
        public static bool IsProjectManagedWithGit()
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
        /// Get the version of a Unity package by its name.
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public static string GetPackageVersion(string packageName)
        {
            var request = Client.List(true, true); // This requests the list of packages
            while (!request.IsCompleted) { } // Wait until the request is completed

            if (request.Status == StatusCode.Success)
            {
                foreach (var package in request.Result)
                {
                    if (package.name == packageName)
                    {
                        return package.version;
                    }
                }
            }
            else if (request.Status >= StatusCode.Failure)
            {
                Debug.LogError("Failed to get package version.");
            }
            // Return an empty string or null if the package is not found
            return null;
        }

        /// <summary>
        /// Get package information by package name
        /// </summary>
        public static UnityEditor.PackageManager.PackageInfo GetPackageInfo(string packageName)
        {
            var request = Client.List(true, true);
            while (!request.IsCompleted) { }
            if (request.Status == StatusCode.Success) { return request.Result.FirstOrDefault(pkg => pkg.name == packageName); }
            return null;
        }

        /// <summary>
        /// Add a Unity package by its name and version.
        /// </summary>
        /// <param name="packageNameWithVersion"></param>
        /// <returns></returns>
        public static bool AddUnityPackage(string packageNameWithVersion)
        {
            // Separate the package name and version
            var packageName = packageNameWithVersion.Split('@')[0];
            var version = packageNameWithVersion.Split('@').Length > 1 ? packageNameWithVersion.Split('@')[1] : null;

            // If the package is not Unity official, add a scoped registry for OpenUPM
            if (!packageName.StartsWith("com.unity."))
            {
                AddScopedRegistryOfOpenUpmPackage(packageName);
            }

            // Add the package
            var request = UnityEditor.PackageManager.Client.Add(packageNameWithVersion);
            while (!request.IsCompleted) { }
            if (request.Error != null)
            {
                Debug.LogError(request.Error.message);
                return false;
            }
            return true;
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