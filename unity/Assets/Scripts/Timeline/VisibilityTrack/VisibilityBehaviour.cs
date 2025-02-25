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

using UnityEngine;
using UnityEngine.Playables;

#endregion

/// <summary>
/// A behaviour that manages the visibility of a target GameObject during playback.
/// </summary>
public class VisibilityBehaviour : PlayableBehaviour
{
    public GameObject targetObject;
    public GameObject transformGizmoPrefab;
    public GameObject orientationReference; // The orientation reference for the transform gizmo to follow

    private GameObject _transformGizmo;
    private bool _isVisible = false;

    /// <summary>
    /// Called when the behaviour starts playing, setting the visibility to true.
    /// </summary>
    /// <param name="playable">The playable that is being played.</param>
    /// <param name="info">Frame data for the current frame.</param>
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        SetVisibility(true);
    }

    /// <summary>
    /// Called when the behaviour is paused, setting the visibility to false.
    /// </summary>
    /// <param name="playable">The playable that is being paused.</param>
    /// <param name="info">Frame data for the current frame.</param>
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        SetVisibility(false);
    }

    /// <summary>
    /// Processes each frame of the playable, updating the visibility based on the target object.
    /// </summary>
    /// <param name="playable">The playable being processed.</param>
    /// <param name="info">Frame data for the current frame.</param>
    /// <param name="playerData">Data associated with the player.</param>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (targetObject != null)
        {
            if (_transformGizmo == null && transformGizmoPrefab != null)
                // Instantiate the transform gizmo prefab and parent it to the target object
                _transformGizmo = Object.Instantiate(transformGizmoPrefab, targetObject.transform);

            if (_transformGizmo != null && orientationReference != null)
                // Set the transform gizmo's position and rotation to match the orientation reference
                _transformGizmo.transform.rotation = orientationReference.transform.rotation;

            if (_transformGizmo)
            {
                var currentTime = playable.GetTime();
                var duration = playable.GetDuration();
                var shouldBeVisible = currentTime >= 0 && currentTime <= duration;
                SetVisibility(shouldBeVisible);
            }
        }
        else
        {
            SetVisibility(false);
        }
    }

    /// <summary>
    /// Sets the visibility of the target GameObject.
    /// </summary>
    /// <param name="value">True to make the object visible, false to hide it.</param>
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