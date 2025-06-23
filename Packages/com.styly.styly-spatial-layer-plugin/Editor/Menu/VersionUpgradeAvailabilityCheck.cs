using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

// <summary>
// This script checks for updates of Unity Package Manager (UPM) packages, including their dependencies
// </summary>
// <remarks> 
// It lists all installed packages, identifies their dependencies, and checks for updates in the UPM registry.
// It logs the results in the console, indicating whether updates are available or if the package is up-to-date.
// It also handles indirect dependencies by performing a depth-first search (DFS) to find all related packages and their versions.
// </remarks>
namespace Styly.VisionOs.Plugin
{
    public static class VersionUpgradeAvailabilityCheck
    {
        private static ListRequest listRequest;
        private static readonly List<SearchTask> pendingSearches = new List<SearchTask>();

        private const string MenuCheckUpdates = "Window/Check UPM Package Updates";
        private const string MenuCheckUpdatesWithDeps = "Window/Check UPM Package Updates (incl. Dependencies)";
        [MenuItem(MenuCheckUpdates, false, 2011)]
        public static void CheckPackageUpdates()
        {
            pendingSearches.Clear();
            // Only direct dependencies
            listRequest = Client.List(offlineMode: false, includeIndirectDependencies: false);
            EditorApplication.update += OnListCompleted;
        }

        [MenuItem(MenuCheckUpdatesWithDeps, false, 2011)]
        public static void CheckPackageUpdatesWithDeps()
        {
            pendingSearches.Clear();
            // Get including indirect dependencies
            listRequest = Client.List(/*offlineMode*/ false, /*includeIndirect*/ true);
            EditorApplication.update += OnListCompleted;
        }

        /// <summary>
        /// Hide the menu items if the project is not managed with Git.
        /// This is to prevent showing the menu items when the project is not using Git, as the package updates are typically relevant only for Git-managed projects.
        /// This is done using an internal static class that runs on Unity's load.
        /// </summary>
        [InitializeOnLoad]
        internal static class HideMenuWhenNoGit
        {
            static HideMenuWhenNoGit()
            {
                // Check if the project is managed with Git
                // If not, remove the menu items for checking package updates
                if (PackageManagerUtility.IsProjectManagedWithGit()) return;

                // Remove the menu items for checking package updates
                // This is done using reflection to access the internal UnityEditor.Menu class
                // and remove the menu items, as Unity does not provide a public API to remove menu items.
                EditorApplication.delayCall += RemoveMenus;
            }

            private static void RemoveMenus()
            {
                RemoveMenuItem(MenuCheckUpdates);
                RemoveMenuItem(MenuCheckUpdatesWithDeps);
            }

            private static void RemoveMenuItem(string path)
            {
                // Use reflection to access the internal UnityEditor.Menu class and remove the menu item
                // This is necessary because Unity does not provide a public API to remove menu items.
                var menuType = typeof(EditorApplication).Assembly.GetType("UnityEditor.Menu");
                var remove = menuType?.GetMethod(
                    "RemoveMenuItem",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);   // ← include NonPublic
                remove?.Invoke(null, new object[] { path });
            }
        }

        /// <summary>
        /// Represents a task for searching a package in the UPM registry.
        /// It contains the installed package information, the search request, and whether it is a direct dependency.
        /// It also keeps track of parent packages for indirect dependencies.
        /// </summary>
        private class SearchTask
        {
            public UnityEditor.PackageManager.PackageInfo Installed;
            public SearchRequest Request;

            // Whether it is a direct dependency
            public bool IsDirect;
            // Parent packages (not empty if not a direct dependency)
            public List<string> Parents = new();
        }

        /// <summary>
        /// Callback for when the list request is completed.
        /// This will start the search requests for each package.
        /// </summary>
        private static void OnListCompleted()
        {
            if (!listRequest.IsCompleted) return;
            EditorApplication.update -= OnListCompleted;

            if (listRequest.Status != StatusCode.Success)
            {
                Debug.LogError($"Failed to list packages: {listRequest.Error.message}");
                return;
            }

            // Map all packages
            var all = listRequest.Result.ToDictionary(p => p.name, p => p);

            // Reverse lookup table: dependency → dependent
            var dependentsMap = new Dictionary<string, HashSet<string>>();

            // DFS starting from direct dependency packages
            foreach (var root in all.Values.Where(p => p.isDirectDependency))
            {
                DFS(root.name, root.name, new HashSet<string>());
            }

            void DFS(string current, string root, HashSet<string> visited)
            {
                if (!all.TryGetValue(current, out var info)) return;
                if (!visited.Add(current)) return;        // Prevent loop
                foreach (var dep in info.dependencies)
                {
                    if (!dependentsMap.TryGetValue(dep.name, out var set))
                        dependentsMap[dep.name] = set = new HashSet<string>();
                    set.Add(root);
                    DFS(dep.name, root, visited);
                }
            }

            foreach (var p in all.Values)
            {
                if (p.source is PackageSource.BuiltIn or PackageSource.Local) continue;

                var task = new SearchTask
                {
                    Installed = p,
                    IsDirect = p.isDirectDependency,
                    Parents = dependentsMap.TryGetValue(p.name, out var s)
                                    ? s.ToList()
                                    : new List<string>()
                };
                task.Request = Client.Search(p.name, offlineMode: false);
                pendingSearches.Add(task);
            }

            if (pendingSearches.Count == 0)
            {
                Debug.Log("No searchable packages found.");
                return;
            }

            EditorApplication.update += OnSearchesProgress;
        }

        /// <summary>
        /// Callback for checking the progress of search requests.
        /// This will process completed search requests and remove them from the pending list.
        /// </summary>
        private static void OnSearchesProgress()
        {
            for (int i = pendingSearches.Count - 1; i >= 0; i--)
            {
                var task = pendingSearches[i];
                if (!task.Request.IsCompleted) continue;

                ProcessSearchResult(task);
                pendingSearches.RemoveAt(i);
            }

            if (pendingSearches.Count == 0)
            {
                EditorApplication.update -= OnSearchesProgress;
                Debug.Log("Package-update check finished.");
            }
        }

        /// <summary>
        /// Process the search result for a package.
        /// If the package is not found or the version is up-to-date, it will log a warning.
        /// If an update is available, it will log the details.
        /// </summary>
        /// <param name="task"></param>
        private static void ProcessSearchResult(SearchTask task)
        {
            var installed = task.Installed;
            var searchReq = task.Request;

            if (searchReq.Status != StatusCode.Success)
            {
                // Debug.LogWarning($"Search failed for {installed.name}: {searchReq.Error.message}");
                return;
            }

            if (searchReq.Result.Length == 0)
            {
                Debug.LogWarning($"Package {installed.name} is not in the registry.");
                return;
            }

            var latest = searchReq.Result[0].version;
            var parentInfo = task.IsDirect
                ? ""
                : $"  (dependency of: {string.Join(", ", task.Parents)})";

            if (IsNewerVersion(latest, installed.version))
            {
                Debug.Log($"<color=orange>Update available</color>  {installed.name}: " +
                        $"Installed {installed.version} → Latest {latest}{parentInfo}");
            }
            else
            {
                // Debug.Log($"{installed.name} is up-to-date ({installed.version}).{parentInfo}");
            }
        }

        /// <summary>
        /// Check if the latest version is newer than the current version.
        /// This uses System.Version for comparison, which allows for semantic versioning.
        /// </summary>
        /// <param name="latest"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private static bool IsNewerVersion(string latest, string current)
            => System.Version.TryParse(latest, out var lv) &&
                System.Version.TryParse(current, out var cv) &&
                lv > cv;
    }
}
