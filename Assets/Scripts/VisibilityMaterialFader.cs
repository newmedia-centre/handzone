using System.Collections;
using UnityEngine;

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

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                startColors[i] = renderers[i].material.color;
                endColors[i] = new Color(startColors[i].r, startColors[i].g, startColors[i].b, 0.0f);
            }
        }
    }

    public IEnumerator FadeIn()
    {
        gameObject.SetActive(true);
        if (isFading || renderers == null) yield break;
        isFading = true;
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].material.color = Color.Lerp(endColors[i], startColors[i], elapsedTime / fadeDuration);
                }
            }
            yield return null;
        }
        
        isFading = false;
    }

    public IEnumerator FadeOut()
    {
        if (isFading || renderers == null) yield break;
        isFading = true;
        float elapsedTime = 0.0f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].material.color = Color.Lerp(startColors[i], endColors[i], elapsedTime / fadeDuration);
                }
            }
            yield return null;
        }
        
        isFading = false;
        gameObject.SetActive(false);
    }
}