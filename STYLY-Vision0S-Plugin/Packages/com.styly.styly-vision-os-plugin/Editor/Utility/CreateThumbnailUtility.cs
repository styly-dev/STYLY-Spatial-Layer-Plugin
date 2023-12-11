using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Styly.VisionOs.Plugin
{
    public class CreateThumbnailUtility
    {
        public static void MakeThumbnail(string sourceFilePath, string destFilePath)
        {
            var dirPath = Path.GetDirectoryName(destFilePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            try
            {
                MakeThumbnailForPrefab(sourceFilePath, destFilePath);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        
        private static bool MakeThumbnail(GameObject unit, string savePath, int width, int height)
        {
            int layerNo;

            unit.transform.eulerAngles = new Vector3(-10.0f, -60.0f, 15.0f);
            layerNo = UnityGameObjectThumbnailLayerClass.CreateLayer();

            if (layerNo == 7)
            {
                ShowErrorDialog("There is no space in the layer, creation of thumbnail failed.");
                return false;
            }

            unit.SetLayer(layerNo);

            Bounds maxBounds = new Bounds(Vector3.zero, Vector3.zero);

            Component[] meshFilterList;
            meshFilterList = unit.gameObject.GetComponentsInChildren(typeof(MeshFilter));

            Component[] SkinnedMeshRendererList;
            SkinnedMeshRendererList = unit.gameObject.GetComponentsInChildren(typeof(SkinnedMeshRenderer));

            if (meshFilterList != null)
            {
                foreach (MeshFilter child in meshFilterList)
                {
                    Transform t = child.transform;
                    if (child.sharedMesh != null)
                    {
                        Bounds bounds = child.sharedMesh.bounds;
                        Vector3 b2Size = new Vector3(bounds.size.x * t.lossyScale.x, bounds.size.y * t.lossyScale.y,
                            bounds.size.z * t.lossyScale.z);
                        Bounds b2 = new Bounds(t.localToWorldMatrix.MultiplyPoint(bounds.center),
                            b2Size);
                        maxBounds.Encapsulate(b2);
                    }
                }
            }

            if (maxBounds.size.x == 0f && maxBounds.size.y == 0f)
            {
                foreach (SkinnedMeshRenderer child in SkinnedMeshRendererList)
                {
                    Transform t = child.transform;
                    Bounds bounds = child.localBounds;
                    Vector3 b2Size = new Vector3(bounds.size.x * t.lossyScale.x, bounds.size.y * t.lossyScale.y,
                        bounds.size.z * t.lossyScale.z);
                    Bounds b2 = new Bounds(t.localToWorldMatrix.MultiplyPoint(bounds.center),
                        b2Size);
                    maxBounds.Encapsulate(b2);
                }
            }

            float cameraSize;
            cameraSize = System.Math.Max(maxBounds.extents.x, maxBounds.extents.y);

            GameObject secondCamera;
            secondCamera = new GameObject("SecondCamera");
            var sc = secondCamera.AddComponent<UnityEngine.Camera>();

            for (int i = 0; i <= 31; i++)
            {
                sc.cullingMask &= (1 << i);
            }

            sc.cullingMask |= (1 << layerNo);

            secondCamera.transform.position = new Vector3(0, 0, -1000);
            secondCamera.transform.LookAt(new Vector3(maxBounds.center.x, maxBounds.center.y, maxBounds.center.z));

            sc.nearClipPlane = 0.001f;
            sc.farClipPlane = 3000f;
            sc.orthographic = true;
            sc.orthographicSize = cameraSize * 1.5f;

            sc.clearFlags = CameraClearFlags.SolidColor;
            sc.backgroundColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);

            CaptureAndSaveThumbnail(sc, savePath, width, height);

            GameObject.DestroyImmediate(unit);
            GameObject.DestroyImmediate(secondCamera);

            return true;
        }

        private static void MakeThumbnailForPrefab(string assetPath, string thumbnailPath)
        {
            GameObject targetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            GameObject unit =
                UnityEngine.Object.Instantiate(targetObj, Vector3.zero, Quaternion.identity) as GameObject;
            if (!MakeThumbnail(unit, thumbnailPath, Config.ThumbnailWidth, Config.ThumbnailHeight))
            {
                throw new InvalidOperationException("MakeThumbnail is failed.");
            }
        }

        private static void CaptureAndSaveThumbnail(Camera camera, string savePath, int width, int height)
        {
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            camera.targetTexture = renderTexture;
            camera.Render();
            RenderTexture.active = renderTexture;
            Texture2D texture2D =
                new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            byte[] bytes = texture2D.EncodeToPNG();
            try
            {
                System.IO.File.WriteAllBytes(savePath, bytes);
            }
            catch (System.Exception ex)
            {
                ShowErrorDialog("Failed to export thumbnail. " + ex.Message);
            }

            camera.targetTexture = null;
            RenderTexture.active = null;
            renderTexture.Release();
        }

        private static void ShowErrorDialog(string description)
        {
            ShowDialog("Asset Build failed", description, "OK");
        }

        private static void ShowDialog(string title, string description, string buttonName)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog(title, description, buttonName);
        }
    }
    
    public static class UnityGameObjectThumbnailLayerClass
    {
        public static void SetLayer(this GameObject gameObject, int layerNo, bool needSetChildrens = true)
        {
            if (gameObject == null)
            {
                return;
            }

            gameObject.layer = layerNo;

            if (!needSetChildrens)
            {
                return;
            }

            foreach (Transform childTransform in gameObject.transform)
            {
                SetLayer(childTransform.gameObject, layerNo, needSetChildrens);
            }
        }

        const string useCaptureTag = "SUITE.STYLY.CC.USE_CAPTURE";

        public static int CreateLayer()
        {
            int layerNumber = 7;
            SerializedObject tagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            for (int i = 31; i > 0; i--)
            {
                if ((layers.GetArrayElementAtIndex(i).stringValue == "" ||
                     layers.GetArrayElementAtIndex(i).stringValue.Equals(useCaptureTag)) && i > 7)
                {
                    layers.GetArrayElementAtIndex(i).stringValue = useCaptureTag;
                    layerNumber = i;
                    break;
                }

                if (i == 7)
                {
                    layerNumber = i;
                    break;
                }
            }

            tagManager.ApplyModifiedProperties();
            return layerNumber;
        }
    }
}