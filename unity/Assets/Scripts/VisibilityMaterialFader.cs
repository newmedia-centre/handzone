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
using UnityEngine;

#endregion

public class VisibilityMaterialFader : MonoBehaviour
{
    public float fadeDuration = 1.0f;

    private Renderer[] renderers;
    private Color[] startColors;
    private Color[] endColors;
    private bool isFading = false;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogError("No renderers found on the game object or its children.");
            return;
        }

        startColors = new Color[renderers.Length];
        endColors = new Color[renderers.Length];

        for (var i = 0; i < renderers.Length; i++)
            if (renderers[i] != null)
            {
                startColors[i] = renderers[i].material.color;
                endColors[i] = new Color(startColors[i].r, startColors[i].g, startColors[i].b, 0.0f);
            }
    }

    public IEnumerator FadeIn()
    {
        gameObject.SetActive(true);
        if (isFading || renderers == null) yield break;
        isFading = true;
        var elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            for (var i = 0; i < renderers.Length; i++)
                if (renderers[i] != null)
                    renderers[i].material.color = Color.Lerp(endColors[i], startColors[i], elapsedTime / fadeDuration);
            yield return null;
        }

        isFading = false;
    }

    public IEnumerator FadeOut()
    {
        if (isFading || renderers == null) yield break;
        isFading = true;
        var elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            for (var i = 0; i < renderers.Length; i++)
                if (renderers[i] != null)
                    renderers[i].material.color = Color.Lerp(startColors[i], endColors[i], elapsedTime / fadeDuration);
            yield return null;
        }

        isFading = false;
        gameObject.SetActive(false);
    }
}