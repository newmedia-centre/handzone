using UnityEngine;
using UnityEngine.Playables;

public class HighlightControlMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        int inputCount = playable.GetInputCount();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight > 0.5f)
            {
                Playable inputPlayable = playable.GetInput(i);
                if (inputPlayable.GetPlayableType() == typeof(ScriptPlayable<HighlightControlBehaviour>))
                {
                    ScriptPlayable<VisibilityBehaviour> visibilityPlayable = (ScriptPlayable<VisibilityBehaviour>)inputPlayable;
                    VisibilityBehaviour visibilityBehaviour = visibilityPlayable.GetBehaviour();
                    visibilityBehaviour.ProcessFrame(playable, info, playerData);
                }
                else if (inputPlayable.GetPlayableType() == typeof(ScriptPlayable<TranslatableHighlightControlBehaviour>))
                {
                    ScriptPlayable<TranslatableHighlightControlBehaviour> translatablePlayable = (ScriptPlayable<TranslatableHighlightControlBehaviour>)inputPlayable;
                    TranslatableHighlightControlBehaviour translatableBehaviour = translatablePlayable.GetBehaviour();
                    translatableBehaviour.ProcessFrame(playable, info, playerData);
                }
            }
        }
    }
}