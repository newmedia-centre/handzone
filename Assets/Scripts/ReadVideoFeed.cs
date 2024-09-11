using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReadVideoFeed : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private List<CameraFeed> _cameraFeeds = new();
    private int _currentCameraFeedIndex;
    
    /// <summary>
    /// This struct is used to store the camera name and the texture of the camera feed.
    /// </summary>
    private class CameraFeed
    {
        public string CameraName;
        public Texture2D Texture2D;
    }
    
    /// <summary>
    /// This method is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// It initializes the MeshRenderer component and subscribes to the OnCameraFeed event from the WebClient.
    /// </summary>
    private void OnEnable()
    {
        if (SessionClient.Instance)
        {
            SessionClient.Instance.OnCameraFeed += UpdateVideoFeed;
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
            SessionClient.Instance.OnCameraFeed -= UpdateVideoFeed;
        }
    }

    private void UpdateVideoFeed(string cameraName, Texture2D texture2D)
    {
        // Check if the camera feed already exists in the list
        foreach (var cameraFeed in _cameraFeeds.Where(cameraFeed => cameraFeed.CameraName == cameraName))
        {
            cameraFeed.Texture2D = texture2D;
            return;
        }

        // Add the camera feed to the list
        _cameraFeeds.Add(new CameraFeed {CameraName = cameraName, Texture2D = texture2D});
        
        // Set the texture of the MeshRenderer component if there is only one camera feed
        if(_cameraFeeds.Count == 1)
        {
            _meshRenderer.material.mainTexture = texture2D;
        }
    }

    public void NextCameraIndex()
    {
        _currentCameraFeedIndex++;
        if (_currentCameraFeedIndex >= _cameraFeeds.Count)
        {
            _currentCameraFeedIndex = 0;
        }
        
        _meshRenderer.material.mainTexture = _cameraFeeds[_currentCameraFeedIndex].Texture2D;
    }
    
    public void PreviousCameraIndex()
    {
        _currentCameraFeedIndex--;
        if (_currentCameraFeedIndex < 0)
        {
            _currentCameraFeedIndex = _cameraFeeds.Count - 1;
        }
        
        _meshRenderer.material.mainTexture = _cameraFeeds[_currentCameraFeedIndex].Texture2D;
    }
}
