using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

[Serializable]
public class VideoControlClip : PlayableAsset, ITimelineClipAsset
{
    public VideoClip clip; // Field to hold the video clip
    public VideoClipType type; // Field to hold the video clip type

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VideoControlBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.clip = clip; // Assign the video clip to the behaviour
        behaviour.type = type; // Assign the video clip type to the behaviour

        // Set the duration of the playable to the duration of the video clip
        if (clip != null)
        {
            playable.SetDuration(clip.length);
        }

        return playable;
    }

    public ClipCaps clipCaps => ClipCaps.All;
}