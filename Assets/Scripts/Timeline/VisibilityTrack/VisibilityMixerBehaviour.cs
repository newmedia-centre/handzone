using UnityEngine;
using UnityEngine.Playables;

public class VisibilityMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        int inputCount = playable.GetInputCount();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight > 0.5f)
            {
                ScriptPlayable<VisibilityBehaviour> inputPlayable = (ScriptPlayable<VisibilityBehaviour>)playable.GetInput(i);
                VisibilityBehaviour input = inputPlayable.GetBehaviour();
                input.ProcessFrame(playable, info, playerData);
            }
        }
    }
}