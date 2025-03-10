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
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

#endregion

public class RadialInteractable : MonoBehaviour
{
    [Header("UI Control")] public Color selectedColor = Color.cyan;
    public Color arrowOriginColor = Color.cyan;
    public Color circleColor = Color.white;
    public Color arrowTargetColor = Color.gray;
    [Range(0, 360)] public float arrowOrigin = 0f;
    [Range(-360, 360)] public float arrowTarget = 90f;

    [Header("Haptic Feedback")]
    [Range(0, 1)]
    public float intensity = 0.2f;

    public float duration = 0.2f;

    [Header("Image References")]
    [SerializeField]
    private Image selectedBackgroundImage;

    [SerializeField] private Image arrowOriginImage;
    [SerializeField] private Image arrowTargetImage;
    [SerializeField] private Image circleImage;


    [FormerlySerializedAs("interactable")]
    [Header("XR Interactable")]
    [SerializeField]
    private RobotJointInteractable interactableSource;

    private Canvas _canvas;
    private Vector3 _originPosition;

    private void Awake()
    {
        _canvas = GetComponentInChildren<Canvas>();
        _canvas.enabled = false;

        interactableSource.hoverEntered.AddListener(ShowCanvas);
        interactableSource.hoverExited.AddListener(HideCanvas);
        interactableSource.selectExited.AddListener(HideCanvas);
        interactableSource.selectEntered.AddListener(SetOriginPosition);

        UpdateRadial();
        CheckImageReferences();
    }

    private void Update()
    {
        // Change the target angle to the offset position of the controller from the starting interactable position
        if (interactableSource.isSelected && interactableSource.isControllerBeyondThreshold)
            if (interactableSource.GetOldestInteractorSelecting() is XRBaseInteractor controllerInteractor)
            {
                // Get the angle from the origin position to the controller position on the XZ plane
                var originPosition = new Vector3(_originPosition.x, 0, _originPosition.z);
                var controllerPosition = new Vector3(controllerInteractor.transform.position.x, 0,
                    controllerInteractor.transform.position.z);
                var originToController = controllerPosition - originPosition;
                var angle = Vector3.SignedAngle(Vector3.forward, originToController, Vector3.up);

                // Adjust the angle to be within the range of -360 to 360 degrees
                if (angle < 0) angle += 360;

                arrowTarget = angle;
            }
    }

    private void LateUpdate()
    {
        if (interactableSource == null)
        {
            Debug.LogError("RadialInteractable: XRBaseInteractable component not found!");
            return;
        }

        if (arrowOriginImage == null || arrowTargetImage == null || selectedBackgroundImage == null)
        {
            Debug.LogError("RadialInteractable: One or more images are not set!");
            return;
        }

        var direction = Mathf.Sign(arrowTarget + arrowOrigin - arrowOrigin);
        var angle = arrowTarget + arrowOrigin - arrowOrigin;
        if (direction < 0)
        {
            angle = 360 + angle;
            selectedBackgroundImage.fillClockwise = true;
            selectedBackgroundImage.fillAmount = 1 - angle / 360f;
        }
        else
        {
            selectedBackgroundImage.fillAmount = angle / 360f;
            selectedBackgroundImage.fillClockwise = false;
        }

        SetLocalRotation(selectedBackgroundImage, arrowOrigin);
        SetLocalRotation(arrowOriginImage, arrowOrigin);
        SetLocalRotation(arrowTargetImage, arrowTarget + arrowOrigin);
    }

    private void SetLocalRotation(Image image, float angle)
    {
        image.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }


    private void CheckImageReferences()
    {
        if (arrowOriginImage == null || arrowTargetImage == null || selectedBackgroundImage == null)
        {
            Debug.LogError("RadialInteractable: One or more images are not set!");
            enabled = false;
        }
    }

    private void ShowCanvas(BaseInteractionEventArgs args)
    {
        if (args.interactorObject is XRBaseInteractor controllerInteractor) _canvas.enabled = true;
    }

    private void HideCanvas(BaseInteractionEventArgs args)
    {
        if (args.interactorObject is XRBaseInteractor controllerInteractor)
            // It should hide if the controller is not hovering anymore and not selected
            if (!interactableSource.isSelected && !interactableSource.isHovered)
                _canvas.enabled = false;
    }

    private void SetOriginPosition(BaseInteractionEventArgs args)
    {
        // Set the origin position to the controller position on select
        if (args.interactorObject is XRBaseInteractor controllerInteractor)
        {
            arrowOrigin = 0;
            arrowTarget = 0;
            _originPosition = controllerInteractor.transform.position;
        }
    }

    // On editor update
    private void OnValidate()
    {
        UpdateRadial();
    }

    private void UpdateRadial()
    {
        arrowOriginImage.transform.localEulerAngles = new Vector3(0, 0, arrowOrigin);
        arrowTargetImage.transform.localEulerAngles = new Vector3(0, 0, arrowTarget + arrowOrigin);
        selectedBackgroundImage.color = selectedColor;
        arrowOriginImage.color = arrowOriginColor;
        arrowTargetImage.color = arrowTargetColor;
        circleImage.color = circleColor;
    }
}