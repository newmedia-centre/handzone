using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class TranslatableHighlightControlBehaviour : PlayableBehaviour
{
    public Transform sourceTransform;
    public GameObject targetObject;

    private bool firstFrameHappened;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    public override void OnGraphStart(Playable playable)
    {
        if (targetObject != null)
        {
            originalPosition = targetObject.transform.position;
            originalRotation = targetObject.transform.rotation;
            originalScale = targetObject.transform.localScale;
            targetObject.SetActive(false); // Hide the object initially
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (targetObject == null)
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
            targetObject.transform.position = sourceTransform.position;
            targetObject.transform.rotation = sourceTransform.rotation;
            targetObject.transform.localScale = sourceTransform.localScale;
        }
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (targetObject == null)
        {
            return;
        }

        targetObject.SetActive(true); // Make the object visible
        firstFrameHappened = true;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (targetObject != null)
        {
            targetObject.transform.SetPositionAndRotation(originalPosition, originalRotation);
            targetObject.transform.localScale = originalScale;
            targetObject.SetActive(false); // Hide the object
        }
        firstFrameHappened = false;
    }
}