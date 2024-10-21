using UnityEngine;
using UnityEngine.Playables;

public class VisibilityBehaviour : PlayableBehaviour
{
    public GameObject targetObject;

    private bool previousVisibilityState = false;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        SetVisibility(true);
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        SetVisibility(false);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (targetObject != null)
        {
            double currentTime = playable.GetTime();
            double duration = playable.GetDuration();
            bool shouldBeVisible = currentTime >= 0 && currentTime <= duration;
            if (shouldBeVisible != previousVisibilityState)
            {
                SetVisibility(shouldBeVisible);
                previousVisibilityState = shouldBeVisible;
            }
        }
    }

    private void SetVisibility(bool visible)
    {
        if (targetObject != null)
        {
            var fader = targetObject.GetComponent<VisibilityMaterialFader>();
            if (fader != null)
            {
                if (visible)
                {
                    CoroutineHelper.Instance.StartCoroutine(fader.FadeIn());
                }
                else
                {
                    CoroutineHelper.Instance.StartCoroutine(fader.FadeOut());
                }
            }
            else
            {
                targetObject.SetActive(visible);
            }
        }
    }
}