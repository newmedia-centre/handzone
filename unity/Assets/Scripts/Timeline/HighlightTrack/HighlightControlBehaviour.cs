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
using Object = UnityEngine.Object;

#endregion

/// <summary>
/// Controls the highlighting of a target GameObject using an Outline component.
/// </summary>
/// <remarks>
/// This class manages the outline properties such as mode, color, and width,
/// and handles the enabling and disabling of the outline during playback.
/// </remarks>
[Serializable]
public class HighlightControlBehaviour : PlayableBehaviour
{
    public Outline.Mode mode;
    public Color color;
    [Range(0f, 10f)] public float width;

    public GameObject targetObject;

    private Outline outline;
    private bool firstFrameHappened;

    /// <summary>
    /// Processes the frame for the playable, applying the outline settings if the first frame has occurred.
    /// </summary>
    /// <param name="playable">The playable being processed.</param>
    /// <param name="info">Frame data for the current frame.</param>
    /// <param name="playerData">Data associated with the player.</param>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (targetObject == null) return;

        if (!firstFrameHappened)
        {
            outline = targetObject.GetComponent<Outline>();
            if (outline == null)
            {
                if (targetObject.GetComponent<Renderer>() == null)
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

    /// <summary>
    /// Called when the behaviour is played. Initializes the outline settings.
    /// </summary>
    /// <param name="playable">The playable being played.</param>
    /// <param name="info">Frame data for the current frame.</param>
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (targetObject == null) return;

        outline = targetObject.GetComponent<Outline>();
        if (outline == null)
        {
            if (targetObject.GetComponent<Renderer>() == null)
            {
                Debug.LogWarning("HighlightControlBehaviour: targetObject does not have a Renderer component.");
                return;
            }

            outline = targetObject.AddComponent<Outline>();
        }

        outline.OutlineMode = mode;
        outline.OutlineColor = color;
        outline.OutlineWidth = width;
        outline.enabled = true;

        firstFrameHappened = true;
    }

    /// <summary>
    /// Called when the behaviour is paused. Disables the outline.
    /// </summary>
    /// <param name="playable">The playable being paused.</param>
    /// <param name="info">Frame data for the current frame.</param>
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (outline != null)
        {
            outline.enabled = false;
            firstFrameHappened = false;
        }
    }

    /// <summary>
    /// Called when the playable is destroyed. Cleans up the outline component.
    /// </summary>
    /// <param name="playable">The playable being destroyed.</param>
    public override void OnPlayableDestroy(Playable playable)
    {
        if (outline != null)
        {
            if (Application.isEditor)
                Object.DestroyImmediate(outline);
            else
                Object.Destroy(outline);
        }
    }
}