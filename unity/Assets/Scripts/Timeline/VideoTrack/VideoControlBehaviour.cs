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

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

#endregion

/// <summary>
/// A behaviour that controls video playback in a playable.
/// </summary>
[Serializable]
public class VideoControlBehaviour : PlayableBehaviour
{
    public VideoClip clip; // Make this field public

    [SerializeField] public VideoClipType type;

    private VideoPlayer videoPlayer;
    private bool firstFrameHappened;

    /// <summary>
    /// Processes each frame of the playable, managing video playback and frame updates.
    /// </summary>
    /// <param name="playable">The playable being processed.</param>
    /// <param name="info">Frame data for the current frame.</param>
    /// <param name="playerData">Data associated with the player.</param>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        videoPlayer = playerData as VideoPlayer;

        if (videoPlayer == null) return;

        if (!firstFrameHappened)
        {
            firstFrameHappened = true;
            videoPlayer.clip = clip;
            var clipStartTime = playable.GetTime();
            videoPlayer.frame = (long)(clipStartTime * videoPlayer.frameRate); // Use startTime as an offset
            videoPlayer.Play();
        }

        var clipEndTime = playable.GetTime() + playable.GetDuration();
        if (playable.GetTime() >= clipEndTime) videoPlayer.Pause();

        if (type == VideoClipType.HoldLastFrame)
            videoPlayer.frame = (long)Math.Floor(playable.GetTime() * videoPlayer.frameRate);
    }

    /// <summary>
    /// Called when the behaviour is paused, stopping the video and resetting its state.
    /// </summary>
    /// <param name="playable">The playable that is being paused.</param>
    /// <param name="info">Frame data for the current frame.</param>
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (videoPlayer == null) return;

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

    /// <summary>
    /// Called when the playable is destroyed, ensuring the video is stopped.
    /// </summary>
    /// <param name="playable">The playable that is being destroyed.</param>
    public override void OnPlayableDestroy(Playable playable)
    {
        if (videoPlayer == null) return;

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