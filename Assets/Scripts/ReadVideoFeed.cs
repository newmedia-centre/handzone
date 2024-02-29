using UnityEngine;

public class ReadVideoFeed : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    /// <summary>
    /// This method is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// It initializes the MeshRenderer component and subscribes to the OnCameraFeed event from the WebClient.
    /// </summary>
    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        if (_meshRenderer != null)
        {
            WebClient.OnCameraFeed += LoadVideoFeed;
        }
        else
        {
            Debug.LogError("MeshRenderer component is missing!");
        }
    }

    private void LoadVideoFeed(Texture2D texture2D)
    {
        if (_meshRenderer != null)
        {
            _meshRenderer.material.mainTexture = texture2D;
        }
    }
}
