using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class VisibilityBehaviour : PlayableBehaviour
{
    public GameObject targetObject;
    public GameObject transformGizmoPrefab;

    private GameObject transformGizmo;
    private bool isVisible = false;

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
            if (transformGizmo == null && transformGizmoPrefab != null)
            {
                // Instantiate the transform gizmo prefab and parent it to the target object
                transformGizmo = Object.Instantiate(transformGizmoPrefab, targetObject.transform);
            }

            if (transformGizmo)
            {
                double currentTime = playable.GetTime();
                double duration = playable.GetDuration();
                bool shouldBeVisible = currentTime >= 0 && currentTime <= duration;
                SetVisibility(shouldBeVisible);
            }
        }
        else
        {
            SetVisibility(false);
        }
    }

    private void SetVisibility(bool value)
    {
        if (transformGizmo != null)
        {
            var fader = transformGizmo.GetComponent<VisibilityMaterialFader>();
            if (fader != null)
            {
                if (value && !isVisible)
                {
                    CoroutineHelper.Instance?.StartCoroutine(fader.FadeIn());
                    isVisible = true;
                }
                else if (!value && isVisible)
                {
                    CoroutineHelper.Instance?.StartCoroutine(fader.FadeOut());
                    CoroutineHelper.Instance?.StartCoroutine(DestroyAfterFadeOut(fader));
                    isVisible = false;
                }
            }
            else
            {
                transformGizmo.SetActive(value);
                if (!value)
                {
                    Object.Destroy(transformGizmo);
                    transformGizmo = null;
                }
            }
        }
    }

    private IEnumerator DestroyAfterFadeOut(VisibilityMaterialFader fader)
    {
        yield return new WaitForSeconds(fader.fadeDuration);
        Object.Destroy(transformGizmo);
        transformGizmo = null;
    }
}