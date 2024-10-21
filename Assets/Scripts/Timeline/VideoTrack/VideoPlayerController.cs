using UnityEngine;
using UnityEngine.Video;

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
