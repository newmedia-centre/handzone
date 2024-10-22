using UnityEngine;
using UnityEngine.Playables;

public class VisibilityBehaviour : PlayableBehaviour
{
    public GameObject targetObject;

    private bool previousVisibilityState = false;
    private GameObject transformGizmo;
    
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
            if (transformGizmo == null)
            {
                // Find the direct child named "TransformGizmo" and store it
                foreach (Transform child in targetObject.transform)
                {
                    if (child.name == "TransformGizmo(Clone)")
                    {
                        transformGizmo = child.gameObject;
                        break;
                    }
                }
            }

            if (transformGizmo)
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
    }

    private void SetVisibility(bool visible)
    {
        if (transformGizmo != null)
        {
            var fader = transformGizmo.GetComponent<VisibilityMaterialFader>();
            if (fader != null)
            {
                if (visible)
                {
                    CoroutineHelper.Instance?.StartCoroutine(fader.FadeIn());
                }
                else
                {
                    CoroutineHelper.Instance?.StartCoroutine(fader.FadeOut());
                }
            }
            else
            {
                transformGizmo.SetActive(visible);
            }
        }
    }
}