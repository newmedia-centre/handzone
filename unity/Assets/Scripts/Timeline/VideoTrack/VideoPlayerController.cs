// Copyright 2024 NewMedia Centre - Delft University of Technology
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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