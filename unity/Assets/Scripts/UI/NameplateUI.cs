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
        // tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0.0f);
        // lineRenderer.startColor = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g,
        //    lineRenderer.startColor.b, 0.0f);
        //lineRenderer.endColor =
        //    new Color(lineRenderer.endColor.r, lineRenderer.endColor.g, lineRenderer.endColor.b, 0.0f);
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
        if (gameObject.activeSelf == false) return;

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