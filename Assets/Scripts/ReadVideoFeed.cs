using System.Collections.Generic;
using UnityEngine;

public class ReadVideoFeed : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private List<CameraFeed> _cameraFeeds = new();
    private int _currentCameraFeedIndex;
    private bool _isFeedAvailable = false;

    private class CameraFeed
    {
        public string CameraName;
        public Texture2D Texture2D;
    }

    private void OnEnable()
    {
        if (SessionClient.Instance)
        {
            SessionClient.Instance.OnCameraFeed += UpdateVideoFeed;
        }

        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer == null)
        {
            Debug.LogError("MeshRenderer component is missing!");
        }
    }

    private void OnDisable()
    {
        if (SessionClient.Instance)
        {
            SessionClient.Instance.OnCameraFeed -= UpdateVideoFeed;
        }
    }

    private void UpdateVideoFeed(string cameraName, Texture2D texture2D)
    {
        CameraFeed existingCameraFeed = _cameraFeeds.Find(cf => cf.CameraName == cameraName);
        if (existingCameraFeed != null)
        {
            existingCameraFeed.Texture2D = texture2D;
        }
        else
        {
            _cameraFeeds.Add(new CameraFeed { CameraName = cameraName, Texture2D = texture2D });
            _isFeedAvailable = true;
        }

        if (_cameraFeeds.Count == 1)
        {
            _meshRenderer.material.mainTexture = texture2D;
        }
    }

    public void NextCameraIndex()
    {
        if (!_isFeedAvailable)
        {
            return;
        }

        _currentCameraFeedIndex = (_currentCameraFeedIndex + 1) % _cameraFeeds.Count;
        _meshRenderer.material.mainTexture = _cameraFeeds[_currentCameraFeedIndex].Texture2D;
    }

    public void PreviousCameraIndex()
    {
        if (!_isFeedAvailable)
        {
            return;
        }

        _currentCameraFeedIndex = (_currentCameraFeedIndex - 1 + _cameraFeeds.Count) % _cameraFeeds.Count;
        _meshRenderer.material.mainTexture = _cameraFeeds[_currentCameraFeedIndex].Texture2D;
    }
}
