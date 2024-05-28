using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class HighlightControlClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField]
    private HighlightControlBehaviour template = new HighlightControlBehaviour();
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<HighlightControlBehaviour>.Create(graph, template);
    }

    public ClipCaps clipCaps => ClipCaps.None;
}
