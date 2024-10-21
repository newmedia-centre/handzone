using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class UI_Interactable : XRBaseInteractable
{
    UIDocument uiDocument;
    private XRRayInteractor currentRayInteractor;

    private void OnEnable()
    {
        base.OnEnable();
        
        uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument is missing.");
            return;
        }
        
        hoverEntered.AddListener(HandleHover);
        selectEntered.AddListener(HandleSelect);
    }
    
    private void OnDisable()
    {
        base.OnDisable();
        
        hoverEntered.RemoveListener(HandleHover);
        selectEntered.RemoveListener(HandleSelect);
    }

    private void HandleSelect(SelectEnterEventArgs arg0)
    {
        if (arg0.interactorObject is XRRayInteractor rayInteractor)
        {
            currentRayInteractor = rayInteractor;
        }
    }

    private void HandleHover(HoverEnterEventArgs arg0)
    {
        if (arg0.interactorObject is XRRayInteractor rayInteractor)
        {
            currentRayInteractor = rayInteractor;
        }
    }

    private void Update()
    {
        if (currentRayInteractor == null) return;
        
        uiDocument.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPos) =>
        {
            var invalidPosition = new Vector2(float.NaN, float.NaN);

            if (currentRayInteractor.TryGetCurrent3DRaycastHit(out var hit))
            {
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.y = 1 - pixelUV.y;
                pixelUV.x *= uiDocument.panelSettings.targetTexture.width;
                pixelUV.y *= uiDocument.panelSettings.targetTexture.height;

                return pixelUV;
            }
            
            return invalidPosition;
        });
    }
}