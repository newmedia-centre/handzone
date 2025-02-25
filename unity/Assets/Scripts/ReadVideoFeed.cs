/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

#region

using System.Collections.Generic;
using UnityEngine;

#endregion

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
        if (SessionClient.Instance) SessionClient.Instance.OnCameraFeed += UpdateVideoFeed;

        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer == null) Debug.LogError("MeshRenderer component is missing!");
    }

    private void OnDisable()
    {
        if (SessionClient.Instance) SessionClient.Instance.OnCameraFeed -= UpdateVideoFeed;
    }

    private void UpdateVideoFeed(string cameraName, Texture2D texture2D)
    {
        var existingCameraFeed = _cameraFeeds.Find(cf => cf.CameraName == cameraName);
        if (existingCameraFeed != null)
        {
            existingCameraFeed.Texture2D = texture2D;
        }
        else
        {
            _cameraFeeds.Add(new CameraFeed { CameraName = cameraName, Texture2D = texture2D });
            _isFeedAvailable = true;
        }

        if (_cameraFeeds.Count == 1) _meshRenderer.material.mainTexture = texture2D;
    }

    public void NextCameraIndex()
    {
        if (!_isFeedAvailable) return;

        _currentCameraFeedIndex = (_currentCameraFeedIndex + 1) % _cameraFeeds.Count;
        _meshRenderer.material.mainTexture = _cameraFeeds[_currentCameraFeedIndex].Texture2D;
    }

    public void PreviousCameraIndex()
    {
        if (!_isFeedAvailable) return;

        _currentCameraFeedIndex = (_currentCameraFeedIndex - 1 + _cameraFeeds.Count) % _cameraFeeds.Count;
        _meshRenderer.material.mainTexture = _cameraFeeds[_currentCameraFeedIndex].Texture2D;
    }
}