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