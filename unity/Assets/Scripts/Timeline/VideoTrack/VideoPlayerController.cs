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

using UnityEngine;
using UnityEngine.Video;

#endregion

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(MeshRenderer))]
public class VideoPlayerController : MonoBehaviour
{
    private VideoPlayer _videoPlayer;
    private MeshRenderer _meshRenderer;
    private Color _initialColor;

    private void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _initialColor = _meshRenderer.material.color;
    }

    private void OnEnable()
    {
        _videoPlayer.started += VideoPlayerHandler;
    }

    private void OnDisable()
    {
        _videoPlayer.started -= VideoPlayerHandler;
    }

    private void VideoPlayerHandler(VideoPlayer source)
    {
        _meshRenderer.material.color = source.isPlaying ? Color.white : _initialColor;
    }
}