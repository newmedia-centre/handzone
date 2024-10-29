using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(1, 1, 0)]
[TrackClipType(typeof(HighlightControlClip))]
[TrackClipType(typeof(TranslatableHighlightControlClip))]
public class HighlightControlTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<HighlightControlBehaviour>.Create(graph, inputCount);
    }
}