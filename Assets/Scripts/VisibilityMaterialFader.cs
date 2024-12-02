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