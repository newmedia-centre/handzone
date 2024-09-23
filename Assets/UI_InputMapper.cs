using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class UI_InputMapper : MonoBehaviour
{
    UIDocument uiDocument;
    public List<XRRayInteractor> rayInteractors;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null || rayInteractors == null || rayInteractors.Count == 0)
        {
            Debug.LogError("UIDocument or XRRayInteractors are missing.");
            return;
        }

        uiDocument.rootVisualElement.Q<Button>("button_test").clicked += () => Debug.Log("Button clicked");

        uiDocument.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPos) =>
        {
            Debug.Log("SetScreenToPanelSpaceFunction called with screenPos: " + screenPos);
            var invalidPosition = new Vector2(float.NaN, float.NaN);

            foreach (var rayInteractor in rayInteractors)
            {
                if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
                {
                    Vector2 pixelUV = hit.textureCoord;

                    pixelUV.y = 1 - pixelUV.y;
                    pixelUV.x *= uiDocument.panelSettings.targetTexture.width;
                    pixelUV.y *= uiDocument.panelSettings.targetTexture.height;

                    Debug.Log("Converted pixelUV: " + pixelUV);
                    return pixelUV;
                }
            }

            return invalidPosition;
        });
    }
}