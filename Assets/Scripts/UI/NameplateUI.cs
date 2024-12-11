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

using System.Collections;
using TMPro;
using UnityEngine;

#endregion

/// <summary>
/// Manages the display of a nameplate UI element that follows a target object.
/// </summary>
public class NameplateUI : MonoBehaviour
{
    public Vector3 OffsetEnd = new(0.08f, 0.0f, 0.0f);
    public Vector3 OffsetStart = new(0.0f, 0.0f, 0.0f);
    public GameObject Target;
    public float DisplayLerpSpeed = 3.0f;

    /// <summary>
    /// Sets the text displayed on the nameplate.
    /// </summary>
    public string DisplayLabel
    {
        set => tmp.text = value;
    }

    [SerializeField] private TextMeshPro tmp;
    [SerializeField] private LineRenderer lineRenderer;

    private bool _isShowing = true;
    private Coroutine visibilityCoroutine;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the text and line renderer components.
    /// </summary>
    private void Awake()
    {
        tmp = GetComponentInChildren<TextMeshPro>();
        lineRenderer = GetComponentInChildren<LineRenderer>();

        // Set initial alpha to 0
        tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0.0f);
        lineRenderer.startColor = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g,
            lineRenderer.startColor.b, 0.0f);
        lineRenderer.endColor =
            new Color(lineRenderer.endColor.r, lineRenderer.endColor.g, lineRenderer.endColor.b, 0.0f);
    }

    /// <summary>
    /// Called once per frame.
    /// Updates the position and rotation of the nameplate based on the target's position.
    /// </summary>
    private void Update()
    {
        if (_isShowing == false)
            return;

        var localPosition = Target.transform.TransformPoint(OffsetEnd);
        transform.position = localPosition;
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        var targetCenter = Target.GetComponent<Collider>().bounds.center;
        lineRenderer.SetPosition(0, targetCenter + OffsetStart);
        lineRenderer.SetPosition(1, transform.position);
    }

    /// <summary>
    /// Shows the nameplate UI.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true); // Ensure the game object is active
        if (visibilityCoroutine != null) StopCoroutine(visibilityCoroutine);
        visibilityCoroutine = StartCoroutine(LerpVisibility(true));
    }

    /// <summary>
    /// Hides the nameplate UI.
    /// </summary>
    public void Hide()
    {
        if(gameObject.activeSelf == false) return;
        
        if (visibilityCoroutine != null) StopCoroutine(visibilityCoroutine);
        visibilityCoroutine = StartCoroutine(LerpVisibility(false));
    }

    /// <summary>
    /// Smoothly changes the visibility of the nameplate UI.
    /// </summary>
    /// <param name="targetVisibility">The target visibility state.</param>
    /// <returns>An enumerator for the coroutine.</returns>
    private IEnumerator LerpVisibility(bool targetVisibility)
    {
        var elapsedTime = 0.0f;

        var startColor = tmp.color;
        var endColor = tmp.color;
        endColor.a = targetVisibility ? 1.0f : 0.0f;

        while (elapsedTime < DisplayLerpSpeed)
        {
            elapsedTime += Time.deltaTime;
            tmp.color = Color.Lerp(startColor, endColor, elapsedTime / DisplayLerpSpeed);
            lineRenderer.endColor = Color.Lerp(startColor, endColor, elapsedTime / DisplayLerpSpeed);
            yield return null;
        }

        tmp.color = endColor;
        _isShowing = targetVisibility;
        if (!targetVisibility) gameObject.SetActive(false); // Deactivate the game object after hiding
    }
}