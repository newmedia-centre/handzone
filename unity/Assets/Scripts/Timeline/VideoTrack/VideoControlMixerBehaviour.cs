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
using UnityEngine.Timeline;
using UnityEngine.Video;

#endregion

/// <summary>
/// Synchronizes the bound VideoPlayer with the active Timeline clip.
/// </summary>
public class VideoControlMixerBehaviour : PlayableBehaviour
{
    public TimelineClip[] clips = Array.Empty<TimelineClip>();

    private VideoClipType _lastClipType = VideoClipType.Default;
    private VideoPlayer _videoPlayer;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        _videoPlayer = playerData as VideoPlayer;
        if (_videoPlayer == null)
            return;

        ConfigureVideoPlayer(_videoPlayer);

        if (!TryGetActiveClip(playable, out var timelineClip, out var clipAsset))
        {
            ApplyInactiveState(_videoPlayer);
            return;
        }

        var sourceTime = GetSourceTime(playable, timelineClip, clipAsset.clip);
        var targetFrame = GetTargetFrame(_videoPlayer, clipAsset.clip, sourceTime);
        var director = playable.GetGraph().GetResolver() as PlayableDirector;
        var isTimelinePlaying = director != null
            ? director.state == PlayState.Playing
            : info.effectivePlayState == PlayState.Playing;
        var shouldPlayVideo = isTimelinePlaying && clipAsset.type != VideoClipType.HoldLastFrame;
        var shouldSeek = Math.Abs(_videoPlayer.frame - targetFrame) > 1;

        EnsureActiveClip(_videoPlayer, clipAsset.clip, targetFrame);

        if (shouldSeek)
            SeekToFrame(_videoPlayer, targetFrame, sourceTime);

        if (shouldPlayVideo)
        {
            _videoPlayer.playbackSpeed = Mathf.Max(0.0001f, (float)timelineClip.timeScale);

            if (!_videoPlayer.isPlaying)
                _videoPlayer.Play();
        }
        else if (_videoPlayer.isPlaying)
        {
            _videoPlayer.Pause();
        }

        _lastClipType = clipAsset.type;
    }

    public override void OnGraphStop(Playable playable)
    {
        if (_videoPlayer == null)
            return;

        _videoPlayer.Stop();
        _videoPlayer.clip = null;
        _videoPlayer.frame = 0;
    }

    private bool TryGetActiveClip(Playable playable, out TimelineClip timelineClip, out VideoControlClip clipAsset)
    {
        timelineClip = null;
        clipAsset = null;

        var inputCount = Math.Min(playable.GetInputCount(), clips.Length);
        var bestWeight = 0f;

        for (var i = 0; i < inputCount; i++)
        {
            var weight = playable.GetInputWeight(i);
            if (weight <= bestWeight)
                continue;

            var candidate = clips[i];
            var candidateAsset = candidate?.asset as VideoControlClip;
            if (candidateAsset?.clip == null)
                continue;

            bestWeight = weight;
            timelineClip = candidate;
            clipAsset = candidateAsset;
        }

        return timelineClip != null && clipAsset != null;
    }

    private static void ConfigureVideoPlayer(VideoPlayer videoPlayer)
    {
        videoPlayer.playOnAwake = false;
        videoPlayer.skipOnDrop = false;
    }

    private void ApplyInactiveState(VideoPlayer videoPlayer)
    {
        switch (_lastClipType)
        {
            case VideoClipType.ContinueLastFrame:
            case VideoClipType.ContinueLastFrameAndPauseAfter:
            case VideoClipType.HoldLastFrame:
                if (videoPlayer.isPlaying)
                    videoPlayer.Pause();
                break;
            case VideoClipType.PauseAfter:
                if (videoPlayer.isPlaying)
                    videoPlayer.Pause();
                break;
            default:
                videoPlayer.Stop();
                videoPlayer.clip = null;
                videoPlayer.frame = 0;
                break;
        }
    }

    private static double GetSourceTime(Playable playable, TimelineClip timelineClip, VideoClip videoClip)
    {
        var director = playable.GetGraph().GetResolver() as PlayableDirector;
        var directorTime = director != null ? director.time : timelineClip.start;
        var localTimelineTime = Math.Max(0d, directorTime - timelineClip.start);
        var sourceTime = timelineClip.clipIn + (localTimelineTime * timelineClip.timeScale);
        var maxSourceTime = Math.Min(videoClip.length, timelineClip.clipIn + (timelineClip.duration * timelineClip.timeScale));

        return Math.Max(0d, Math.Min(sourceTime, maxSourceTime));
    }

    private static long GetTargetFrame(VideoPlayer videoPlayer, VideoClip videoClip, double sourceTime)
    {
        var frameRate = videoClip.frameRate > 0 ? videoClip.frameRate : videoPlayer.frameRate;
        if (frameRate <= 0)
            return 0;

        var maxFrame = videoClip.frameCount > 0 ? (long)videoClip.frameCount - 1 : long.MaxValue;
        var targetFrame = (long)Math.Floor(sourceTime * frameRate);

        return Math.Max(0, Math.Min(targetFrame, maxFrame));
    }

    private static void EnsureActiveClip(VideoPlayer videoPlayer, VideoClip videoClip, long targetFrame)
    {
        if (videoPlayer.clip == videoClip)
            return;

        videoPlayer.Stop();
        videoPlayer.clip = videoClip;
        videoPlayer.frame = targetFrame;
        videoPlayer.Pause();
    }

    private static void SeekToFrame(VideoPlayer videoPlayer, long targetFrame, double sourceTime)
    {
        var wasPlaying = videoPlayer.isPlaying;
        if (wasPlaying)
            videoPlayer.Pause();

        videoPlayer.time = sourceTime;

        if (videoPlayer.frame == targetFrame)
        {
            if (wasPlaying)
                videoPlayer.Play();

            return;
        }

        videoPlayer.frame = targetFrame;

        if (wasPlaying)
            videoPlayer.Play();
    }
}
