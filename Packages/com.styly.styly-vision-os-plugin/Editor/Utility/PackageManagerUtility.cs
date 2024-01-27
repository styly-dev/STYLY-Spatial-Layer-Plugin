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
        private static PackageManagerUtility instance;

        public static PackageManagerUtility Instance => instance ??= new PackageManagerUtility();

        public string GetManifestJsonFileText()
        {
            var result = Path.GetFullPath("Packages");
            var manifestPath = Path.Combine(result, "manifest.json");

            if (!File.Exists(manifestPath))
            {
                return null;
            }

            var manifestJsonFile = File.ReadAllText(manifestPath);

            return manifestJsonFile;
        }

        private PackageInfo targetPackageInfo;

        private Dictionary<string, PackageInfo> packageInfoDic = new Dictionary<string, PackageInfo>();

        public String GetPackagePath(string packageId)
        {
            IEnumerator t = GetPackageInfoCoroutine(packageId);
            while (t.MoveNext())
            {
                Debug.Log("GetPackagePath waiting...");
            }

            return targetPackageInfo.resolvedPath;
        }

        public string GetPackageVersion(string packageId)
        {
            var t = GetPackageInfoCoroutine(packageId);
            while (t.MoveNext())
            {
                Debug.Log("GetPackagePath waiting...");
            }

            return targetPackageInfo?.version;
        }

        private IEnumerator GetPackageInfoCoroutine(string targetPackage)
        {
            if (packageInfoDic.TryGetValue(targetPackage, out targetPackageInfo))
            {
                yield break;
            }

            var listRequest = UnityEditor.PackageManager.Client.List();

            while (listRequest.Status == StatusCode.InProgress)
            {
                yield return null;
            }

            if (listRequest.Status == StatusCode.Success)
            {
                foreach (var packageInfo in listRequest.Result)
                {
                    Debug.Log(packageInfo.packageId + " : " + packageInfo.resolvedPath);
                    string[] buff = packageInfo.packageId.Split('@');

                    packageInfoDic[buff[0]] = packageInfo;
                }
            }

            if (packageInfoDic.TryGetValue(targetPackage, out targetPackageInfo))
            {
                Debug.Log("Success! :" + targetPackageInfo.name);
            }
        }
    }
}