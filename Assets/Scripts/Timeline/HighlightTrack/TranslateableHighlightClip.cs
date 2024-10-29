using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TranslatableHighlightControlClip : PlayableAsset, ITimelineClipAsset
{
    public ExposedReference<GameObject> targetObjects;
    public ExposedReference<Transform> sourceTransform;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TranslatableHighlightControlBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.targetObject = targetObjects.Resolve(graph.GetResolver());
        behaviour.sourceTransform = sourceTransform.Resolve(graph.GetResolver());
        return playable;
    }

    public ClipCaps clipCaps => ClipCaps.None;
}