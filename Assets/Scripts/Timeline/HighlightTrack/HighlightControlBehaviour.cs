using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class HighlightControlBehaviour : PlayableBehaviour
{
    public Outline.Mode mode;
    public Color color;
    [Range(0f, 10f)]
    public float width;

    public GameObject targetObject;
    private Outline outline;
    private bool firstFrameHappened;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (targetObject == null)
        {
            return;
        }

        if (!firstFrameHappened)
        {
            outline = targetObject.GetComponent<Outline>();
            if (outline == null)
            {
                if(targetObject.GetComponent<Renderer>() == null)
                {
                    Debug.LogWarning("HighlightControlBehaviour: targetObject does not have a Renderer component.");
                    return;
                }
                outline = targetObject.AddComponent<Outline>();
            }

            outline.OutlineMode = mode;
            outline.OutlineColor = color;
            outline.OutlineWidth = width;

            firstFrameHappened = true;
        }
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (outline != null && (info.effectivePlayState == PlayState.Paused || playable.GetDuration() <= playable.GetTime()))
        {
            outline.enabled = false;
            firstFrameHappened = false;
        }
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (outline != null)
        {
            if (Application.isEditor)
            {
                UnityEngine.Object.DestroyImmediate(outline);
            }
            else
            {
                UnityEngine.Object.Destroy(outline);
            }
        }
    }
}