using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(NameplateVisibilityAsset))]
[TrackBindingType(typeof(GameObject))]
public class NameplateVisibilityTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<NameplateVisibilityMixerBehaviour>.Create(graph, inputCount);
    }
}

public class NameplateVisibilityMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        GameObject trackBinding = playerData as GameObject;

        if (!trackBinding)
            return;

        int inputCount = playable.GetInputCount();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<NameplateVisibilityBehaviour> inputPlayable = (ScriptPlayable<NameplateVisibilityBehaviour>)playable.GetInput(i);
            NameplateVisibilityBehaviour input = inputPlayable.GetBehaviour();

            if (inputWeight > 0.5f)
            {
                input.ProcessFrame(playable, info, playerData);
            }
        }
    }
}