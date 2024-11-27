using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class VisibilityBehaviour : PlayableBehaviour
{
    public GameObject targetObject;
    public GameObject transformGizmoPrefab;

    private GameObject _transformGizmo;
    private bool _isVisible = false;

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
            if (_transformGizmo == null && transformGizmoPrefab != null)
            {
                // Instantiate the transform gizmo prefab and parent it to the target object
                _transformGizmo = Object.Instantiate(transformGizmoPrefab, targetObject.transform);
            }

            if (_transformGizmo)
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
        if (_transformGizmo != null)
        {
            var fader = _transformGizmo.GetComponent<VisibilityMaterialFader>();
            if (fader != null)
            {
                if (value && !_isVisible)
                {
                    CoroutineHelper.Instance?.StartCoroutine(fader.FadeIn());
                    _isVisible = true;
                }
                else if (!value && _isVisible)
                {
                    CoroutineHelper.Instance?.StartCoroutine(fader.FadeOut());
                    _isVisible = false;
                }
            }

            _transformGizmo.SetActive(value); 
            _isVisible = value;
        }
    }
}