using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VideoControlClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField]
    public VideoControlBehaviour template = new VideoControlBehaviour();

    public double ClipStart;
    public double ClipEnd;
    public double durationUsed;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        ScriptPlayable<VideoControlBehaviour> playable = ScriptPlayable<VideoControlBehaviour>.Create(graph, template);

        VideoControlBehaviour _behaviour = playable.GetBehaviour();
        _behaviour.ClipStart = ClipStart;
        _behaviour.ClipEnd = ClipEnd;
        _behaviour.durationUsed = durationUsed;
        return playable;
    }
    
    public ClipCaps clipCaps => ClipCaps.None;
}
