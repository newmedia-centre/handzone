using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

[Serializable]
public class VideoControlClip : PlayableAsset, ITimelineClipAsset
{
    public VideoClip clip; // Add this field to hold the video clip

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VideoControlBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.clip = clip; // Assign the video clip to the behaviour

        // Set the duration of the playable to the duration of the video clip
        if (clip != null)
        {
            playable.SetDuration(clip.length);
        }

        return playable;
    }

    public ClipCaps clipCaps => ClipCaps.All;
}