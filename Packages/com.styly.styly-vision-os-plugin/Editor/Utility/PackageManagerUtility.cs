using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Styly.VisionOs.Plugin
{
    public class PackageManagerUtility
    {
        public static string GetPackageVersion(string packageName)
        {
            var request = Client.List(); // This requests the list of packages
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
    }
}