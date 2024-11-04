using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class TranslatableHighlightControlBehaviour : PlayableBehaviour
{
    public Transform sourceTransform;
    public SpriteRenderer targetSpriteRenderer;

    private bool firstFrameHappened;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    public override void OnGraphStart(Playable playable)
    {
        if (targetSpriteRenderer != null)
        {
            originalPosition = targetSpriteRenderer.transform.position;
            originalRotation = targetSpriteRenderer.transform.rotation;
            originalScale = targetSpriteRenderer.transform.localScale;
            targetSpriteRenderer.enabled = false; // Hide the object initially
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (targetSpriteRenderer == null)
        {
            return;
        }

        if (!firstFrameHappened)
        {
            firstFrameHappened = true;
        }

        // Immediately set the object's transform to the sourceTransform's values
        if (sourceTransform != null)
        {
            targetSpriteRenderer.transform.position = sourceTransform.position;
            targetSpriteRenderer.transform.rotation = sourceTransform.rotation;
            targetSpriteRenderer.size = sourceTransform.localScale;
        }
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (targetSpriteRenderer == null)
        {
            return;
        }

        targetSpriteRenderer.enabled = true;
        firstFrameHappened = true;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.transform.SetPositionAndRotation(originalPosition, originalRotation);
            targetSpriteRenderer.size = originalScale;
            targetSpriteRenderer.enabled = false;
        }
        firstFrameHappened = false;
    }
}