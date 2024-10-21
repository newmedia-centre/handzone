using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(VisibilityClip))]
public class VisibilityTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<VisibilityMixerBehaviour>.Create(graph, inputCount);
    }
}