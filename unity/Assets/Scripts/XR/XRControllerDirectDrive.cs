/*
 * The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 */

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Directly drives controller transforms from Input System actions.
/// Use this as a workaround if TrackedPoseDriver is not working.
/// </summary>
public class XRControllerDirectDrive : MonoBehaviour
{
    [Header("Target Transforms")]
    public Transform leftController;
    public Transform rightController;

    [Header("Input Actions")]
    public InputActionProperty leftPosition;
    public InputActionProperty leftRotation;
    public InputActionProperty rightPosition;
    public InputActionProperty rightRotation;

    [Header("Settings")]
    [Tooltip("Ignore the tracking state and always apply position/rotation")]
    public bool ignoreTrackingState = true;
    
    [Tooltip("Enable to use direct drive, disable to use default behavior")]
    public bool enableDirectDrive = true;

    void OnEnable()
    {
        leftPosition.action?.Enable();
        leftRotation.action?.Enable();
        rightPosition.action?.Enable();
        rightRotation.action?.Enable();
    }

    void OnDisable()
    {
        leftPosition.action?.Disable();
        leftRotation.action?.Disable();
        rightPosition.action?.Disable();
        rightRotation.action?.Disable();
    }

    void Update()
    {
        if (!enableDirectDrive) return;

        // Drive left controller
        if (leftController != null)
        {
            var pos = leftPosition.action?.ReadValue<Vector3>();
            var rot = leftRotation.action?.ReadValue<Quaternion>();
            
            if (pos.HasValue && !pos.Value.Equals(Vector3.zero))
            {
                leftController.localPosition = pos.Value;
            }
            if (rot.HasValue)
            {
                leftController.localRotation = rot.Value;
            }
        }

        // Drive right controller
        if (rightController != null)
        {
            var pos = rightPosition.action?.ReadValue<Vector3>();
            var rot = rightRotation.action?.ReadValue<Quaternion>();
            
            if (pos.HasValue && !pos.Value.Equals(Vector3.zero))
            {
                rightController.localPosition = pos.Value;
            }
            if (rot.HasValue)
            {
                rightController.localRotation = rot.Value;
            }
        }
    }

    void OnValidate()
    {
        // Auto-bind if not set
        if (leftPosition.action == null)
        {
            var action = new InputAction("LeftPosition", binding: "<XRController>{LeftHand}/devicePosition");
            leftPosition = new InputActionProperty(action);
        }
        if (leftRotation.action == null)
        {
            var action = new InputAction("LeftRotation", binding: "<XRController>{LeftHand}/deviceRotation");
            leftRotation = new InputActionProperty(action);
        }
        if (rightPosition.action == null)
        {
            var action = new InputAction("RightPosition", binding: "<XRController>{RightHand}/devicePosition");
            rightPosition = new InputActionProperty(action);
        }
        if (rightRotation.action == null)
        {
            var action = new InputAction("RightRotation", binding: "<XRController>{RightHand}/deviceRotation");
            rightRotation = new InputActionProperty(action);
        }
    }
}
