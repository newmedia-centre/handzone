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