using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

[Serializable]
public class VideoControlBehaviour : PlayableBehaviour
{
    public VideoClip clip; // Make this field public

    [SerializeField]
    public VideoClipType type;

    private VideoPlayer videoPlayer;
    private bool firstFrameHappened;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        videoPlayer = playerData as VideoPlayer;

        if (videoPlayer == null)
        {
            return;
        }

        if (!firstFrameHappened)
        {
            firstFrameHappened = true;
            videoPlayer.clip = clip;
            double clipStartTime = playable.GetTime();
            videoPlayer.frame = (long)(clipStartTime * videoPlayer.frameRate); // Use startTime as an offset
            videoPlayer.Play();
        }

        double clipEndTime = playable.GetTime() + playable.GetDuration();
        if (playable.GetTime() >= clipEndTime)
        {
            videoPlayer.Pause();
        }

        if (type == VideoClipType.HoldLastFrame)
        {
            videoPlayer.frame = (long)Math.Floor(playable.GetTime() * videoPlayer.frameRate);
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (videoPlayer == null)
        {
            return;
        }

        firstFrameHappened = false;

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