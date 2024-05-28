using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

[Serializable]
public class VideoControlBehaviour : PlayableBehaviour
{
    [SerializeField]
    private VideoClip clip;

    [SerializeField]
    private double startTime;

    [SerializeField]
    public VideoClipType type;
    
    private VideoPlayer videoPlayer;
    
    private bool firstFrameHappaned;

    public double ClipStart;
    public double ClipEnd;
    public double durationUsed;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        videoPlayer = playerData as VideoPlayer;

        if (videoPlayer == null)
        {
            return;
        }
        
        if (TutorialMenuController.CustomDirectorChange)
        {
            if (videoPlayer.clip != clip)
            {
                videoPlayer.clip = clip;
                videoPlayer.Play();
                videoPlayer.Pause();
            }
            
            if (type == VideoClipType.HoldLastFrame)
            {
                videoPlayer.frame = (long) Math.Floor(durationUsed * videoPlayer.frameRate);
            }
            else
            {
                videoPlayer.frame = (long) Math.Floor((playable.GetTime() + durationUsed) * videoPlayer.frameRate);
            }
        }
        else if (!firstFrameHappaned)
        {
            firstFrameHappaned = true;
            
            switch (type)
            {
                case VideoClipType.Default:
                case VideoClipType.PauseAfter:
                    videoPlayer.clip = clip;
                    videoPlayer.frame = (long)Math.Floor(playable.GetTime() * videoPlayer.frameRate);
                    videoPlayer.Play();
                    break;
                case VideoClipType.ContinueLastFrame:
                case VideoClipType.ContinueLastFrameAndPauseAfter:
                    videoPlayer.frame = (long)Math.Floor((playable.GetTime() + durationUsed) * videoPlayer.frameRate);
                    videoPlayer.Play();
                    break;
            }
        }
    }
    
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (videoPlayer == null)
        {
            return;
        }
        
        firstFrameHappaned = false;

        if (playable.GetTime() >= ClipEnd - ClipStart - 0.02f)
        {
            switch (type)
            {
                case VideoClipType.Default:
                case VideoClipType.ContinueLastFrame:
                    videoPlayer.Stop();

                    videoPlayer.clip = null;
                    videoPlayer.frame = 0;
                    break;
                case VideoClipType.PauseAfter:
                case VideoClipType.ContinueLastFrameAndPauseAfter:
                    videoPlayer.Pause();
                    break;
            }
        }
        else
        {
            videoPlayer.Pause();
        }
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (videoPlayer == null)
        {
            return;
        }
        
        videoPlayer.Stop();
    }
}

public enum VideoClipType
{
    Default,
    PauseAfter,
    HoldLastFrame,
    ContinueLastFrame,
    ContinueLastFrameAndPauseAfter
}
