using System;
using UnityEngine;

public class ReadVideoFeed : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    /// <summary>
    /// This method is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// It initializes the MeshRenderer component and subscribes to the OnCameraFeed event from the WebClient.
    /// </summary>
    private void OnEnable()
    {
        if (SessionClient.Instance)
        {
            SessionClient.Instance.OnCameraFeed += LoadVideoFeed;
        }

        if (TryGetComponent(out _meshRenderer) == false)
        {
            Debug.LogError("MeshRenderer component is missing!");
        }
    }

    private void OnDisable()
    {
        if(SessionClient.Instance)
        {
            SessionClient.Instance.OnCameraFeed -= LoadVideoFeed;
        }
    }

    private void LoadVideoFeed(string cameraName, Texture2D texture2D)
    {
        if (_meshRenderer)
        {
            _meshRenderer.material.mainTexture = texture2D;
        }
    }
}
