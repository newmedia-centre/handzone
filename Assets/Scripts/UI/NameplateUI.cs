using System.Collections;
using UnityEngine;
using TMPro;

public class NameplateUI : MonoBehaviour
{
    public Vector3 Offset = new(0.08f, 0.0f, 0.0f);
    public GameObject Target;
    public float DisplayLerpSpeed = 3.0f;
    public string DisplayLabel
    {
        set => tmp.text = value;
    }

    [SerializeField] private string displayLabel = "Joint";
    [SerializeField] private TextMeshPro tmp;
    [SerializeField] private LineRenderer lineRenderer;

    private bool _isShowing = true;
    private Coroutine visibilityCoroutine;

    private void Awake()
    {
        tmp = GetComponentInChildren<TextMeshPro>();
        lineRenderer = GetComponentInChildren<LineRenderer>();
        
        // Set initial alpha to 0
        tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0.0f);
        lineRenderer.startColor = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, 0.0f);
        lineRenderer.endColor = new Color(lineRenderer.endColor.r, lineRenderer.endColor.g, lineRenderer.endColor.b, 0.0f);
    }

    void Update()
    {
        if (_isShowing == false)
            return;

        Vector3 localPosition = Target.transform.TransformPoint(Offset);
        transform.position = localPosition;
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        Vector3 targetCenter = Target.GetComponent<Collider>().bounds.center;
        lineRenderer.SetPosition(0, targetCenter);
        lineRenderer.SetPosition(1, transform.position);
    }

    public void Show()
    {
        gameObject.SetActive(true); // Ensure the game object is active
        if (visibilityCoroutine != null)
        {
            StopCoroutine(visibilityCoroutine);
        }
        visibilityCoroutine = StartCoroutine(LerpVisibility(true));
    }

    public void Hide()
    {
        if (visibilityCoroutine != null)
        {
            StopCoroutine(visibilityCoroutine);
        }
        visibilityCoroutine = StartCoroutine(LerpVisibility(false));
    }

    private IEnumerator LerpVisibility(bool targetVisibility)
    {

        float elapsedTime = 0.0f;

        Color startColor = tmp.color;
        Color endColor = tmp.color;
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
        if (!targetVisibility)
        {
            gameObject.SetActive(false); // Deactivate the game object after hiding
        }
    }
}