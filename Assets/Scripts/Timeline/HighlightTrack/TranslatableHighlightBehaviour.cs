// Copyright 2024 NewMedia Centre - Delft University of Technology
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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