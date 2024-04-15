using System;
using UnityEngine;

namespace Styly
{
    /// <summary>
    /// This script is intended to be attached to a GameObject with a VideoPlayer component.
    /// </summary>
    public class VideoPlayerHelper : MonoBehaviour
    {
        RenderTexture targetRenderTexture = null;
        string videoURL = null;

        void Start()
        {
            // Check the requirements and get the properties of the VideoPlayer component.
            CheckReqirementAndGetPropertiesOfVideoPlayer();

#if USE_WEBREQUESTVISUALSCRIPTINGNODES
            // Call video cache features if Visual Scripting Nodes for WebRequest package is installed
            UseVideoCacheFeature();
#endif
        }

        /// <summary>
        /// Check the requirements and get the properties of the VideoPlayer component.
        /// </summary>
        void CheckReqirementAndGetPropertiesOfVideoPlayer()
        {
            if (GetComponent<UnityEngine.Video.VideoPlayer>() == null)
            {
                Debug.LogError("[VideoPlayerHelper] VideoPlayer component is not attached to the game object.");
                return;
            }
#if UNITY_VISIONOS
            if (GetComponent<UnityEngine.Video.VideoPlayer>().renderMode != UnityEngine.Video.VideoRenderMode.RenderTexture)
            {
                Debug.LogError("[VideoPlayerHelper] RenderMode is not RenderTexture. RenderTexture needs to be used for visionOS.");
                return;
            }
            else if ((targetRenderTexture = GetComponent<UnityEngine.Video.VideoPlayer>().targetTexture) == null)
            {
                Debug.LogError("[VideoPlayerHelper] RenderTexture is not set as the target texture.");
                return;
            }
#endif
            if (GetComponent<UnityEngine.Video.VideoPlayer>().source == UnityEngine.Video.VideoSource.Url)
            {
                videoURL = GetComponent<UnityEngine.Video.VideoPlayer>().url;
                if (videoURL == String.Empty) { videoURL = null; }
            }
        }

        /// <summary>
        /// MarkDirty is required to update the texture in visionOS.
        /// </summary>
        void MarkDirty_to_RenderTexture()
        {
#if USE_POLYSPATIAL
        Unity.PolySpatial.PolySpatialObjectUtils.MarkDirty(targetRenderTexture);
#endif
        }

        void Update()
        {
#if USE_POLYSPATIAL
        if (targetRenderTexture) { MarkDirty_to_RenderTexture(); }
#endif
        }

#if USE_WEBREQUESTVISUALSCRIPTINGNODES
        /// <summary>
        /// Use video cache feature if Visual Scripting Nodes for WebRequest package is installed
        /// </summary> 
        void UseVideoCacheFeature()
        {
            if (videoURL == null) { return; }
            if (STYLY.Http.CacheUtils.CacheFileExists(videoURL, STYLY.Http.CacheUtils.GetSignedUrlIgnorePatters()))
            {
                // Use cache file if it exists
                string cacheFilePath = STYLY.Http.CacheUtils.GetWebRequestUri(videoURL, true, false, STYLY.Http.CacheUtils.GetSignedUrlIgnorePatters());
                GetComponent<UnityEngine.Video.VideoPlayer>().url = cacheFilePath;
                Debug.Log("[VideoPlayerHelper] Set cached file path to the Video Player URL");
            }
            else
            {
                // Start downloading video file for caching after 10 seconds
                Invoke(nameof(DownloadVideoFileForCaching), 10.0f);
            }
        }

        /// <summary>
        /// Download video file and cache it
        /// </summary>
        void DownloadVideoFileForCaching()
        {
            Debug.Log("[VideoPlayerHelper] Downloading video file for caching...");
            var request = STYLY.Http.Http.Get(videoURL)
                .UseCache(STYLY.Http.CacheType.UseCacheAlways)
                .OnSuccess(response => Debug.Log("[VideoPlayerHelper] Video file is downloaded and cached."))
                .OnError(response => Debug.Log("[VideoPlayerHelper] Failed to download video file:" + response.Error))
                .Send();
        }
#endif
    }
}
