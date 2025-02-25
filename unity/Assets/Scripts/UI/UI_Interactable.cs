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
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

#endregion

public class UI_Interactable : XRBaseInteractable
{
    private UIDocument uiDocument;
    private XRRayInteractor currentRayInteractor;

    protected override void OnEnable()
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

    protected override void OnDisable()
    {
        base.OnDisable();

        hoverEntered.RemoveListener(HandleHover);
        selectEntered.RemoveListener(HandleSelect);
    }

    private void HandleSelect(SelectEnterEventArgs arg0)
    {
        if (arg0.interactorObject is XRRayInteractor rayInteractor) currentRayInteractor = rayInteractor;
    }

    private void HandleHover(HoverEnterEventArgs arg0)
    {
        if (arg0.interactorObject is XRRayInteractor rayInteractor) currentRayInteractor = rayInteractor;
    }

    private void Update()
    {
        if (currentRayInteractor == null) return;

        uiDocument.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPos) =>
        {
            var invalidPosition = new Vector2(float.NaN, float.NaN);

            if (currentRayInteractor.TryGetCurrent3DRaycastHit(out var hit))
            {
                var pixelUV = hit.textureCoord;
                pixelUV.y = 1 - pixelUV.y;
                pixelUV.x *= uiDocument.panelSettings.targetTexture.width;
                pixelUV.y *= uiDocument.panelSettings.targetTexture.height;

                return pixelUV;
            }

            return invalidPosition;
        });
    }
}