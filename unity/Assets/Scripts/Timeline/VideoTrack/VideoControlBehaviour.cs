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