using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class HighlightControlClip : PlayableAsset, ITimelineClipAsset
{
    public ExposedReference<GameObject> targetObjects;
    public Outline.Mode mode;
    public Color color;
    [Range(0f, 10f)]
    public float width;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<HighlightControlBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.targetObject = targetObjects.Resolve(graph.GetResolver());
        behaviour.mode = mode;
        behaviour.color = color;
        behaviour.width = width;
        return playable;
    }

    public ClipCaps clipCaps => ClipCaps.None;
}