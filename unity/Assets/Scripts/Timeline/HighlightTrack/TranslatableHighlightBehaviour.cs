/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

#region

using System;
using UnityEngine;
using UnityEngine.Playables;

#endregion

/// <summary>
/// A behaviour that controls the translation of highlight effects in a playable.
/// </summary>
[Serializable]
public class TranslatableHighlightControlBehaviour : PlayableBehaviour
{
    public Transform sourceTransform;
    public SpriteRenderer targetSpriteRenderer;

    private bool firstFrameHappened;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    /// <summary>
    /// Called when the graph starts, initializing the original transform values.
    /// </summary>
    /// <param name="playable">The playable that is starting.</param>
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

    /// <summary>
    /// Processes each frame of the playable, updating the target sprite renderer's transform.
    /// </summary>
    /// <param name="playable">The playable being processed.</param>
    /// <param name="info">Frame data for the current frame.</param>
    /// <param name="playerData">Data associated with the player.</param>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (targetSpriteRenderer == null) return;

        if (!firstFrameHappened) firstFrameHappened = true;

        // Immediately set the object's transform to the sourceTransform's values
        if (sourceTransform != null)
        {
            targetSpriteRenderer.transform.position = sourceTransform.position;
            targetSpriteRenderer.transform.rotation = sourceTransform.rotation;
            targetSpriteRenderer.size = sourceTransform.localScale;
        }
    }

    /// <summary>
    /// Called when the behaviour starts playing, enabling the target sprite renderer.
    /// </summary>
    /// <param name="playable">The playable that is being played.</param>
    /// <param name="info">Frame data for the current frame.</param>
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (targetSpriteRenderer == null) return;

        targetSpriteRenderer.enabled = true;
        firstFrameHappened = true;
    }

    /// <summary>
    /// Called when the behaviour is paused, resetting the target sprite renderer's transform.
    /// </summary>
    /// <param name="playable">The playable that is being paused.</param>
    /// <param name="info">Frame data for the current frame.</param>
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