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