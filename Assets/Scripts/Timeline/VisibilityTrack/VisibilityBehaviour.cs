using UnityEngine;
using UnityEngine.Playables;

public class VisibilityBehaviour : PlayableBehaviour
{
    public GameObject targetObject;
    public bool isVisible;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (targetObject != null)
        {
            var fader = targetObject.GetComponent<VisibilityMaterialFader>();
            if (fader != null)
            {
                CoroutineHelper.Instance.StartCoroutine(fader.FadeIn());
            }
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (targetObject != null)
        {
            var fader = targetObject.GetComponent<VisibilityMaterialFader>();
            if (fader != null)
            {
                CoroutineHelper.Instance.StartCoroutine(fader.FadeOut());
            }
        }
    }
}