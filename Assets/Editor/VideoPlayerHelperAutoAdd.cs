using UnityEngine;
using UnityEditor;
using UnityEngine.Video;
using Styly;

[InitializeOnLoad]
public static class VideoPlayerHelperAutoAdd
{
    static VideoPlayerHelperAutoAdd()
    {
        // Subscribe to the event that is called when a component is added
        ObjectFactory.componentWasAdded += OnComponentAdded;
    }

    private static void OnComponentAdded(Component component)
    {
        // Check if the added component is a VideoPlayer
        if (component is VideoPlayer)
        {
            // Get the GameObject the VideoPlayer was added to
            GameObject gameObject = component.gameObject;

            // Check if the desired script is already attached
            if (gameObject.GetComponent<VideoPlayerHelper>() == null)
            {
                // Add the script if it's not already attached
                gameObject.AddComponent<VideoPlayerHelper>();
            }
        }
    }
}