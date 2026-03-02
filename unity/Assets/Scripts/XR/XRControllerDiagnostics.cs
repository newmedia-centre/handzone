/*
 * The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 */

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

/// <summary>
/// Diagnostic script to help identify XR controller tracking issues.
/// Attach this to the XR Origin or any active GameObject in the scene.
/// </summary>
public class XRControllerDiagnostics : MonoBehaviour
{
    [Header("Controller Transforms (assign in inspector)")]
    public Transform leftController;
    public Transform rightController;

    [Header("Debug Output")]
    public bool showDebugGUI = true;
    public bool logToConsole = true;

    private string _debugInfo = "";
    private InputAction _leftPositionAction;
    private InputAction _leftRotationAction;
    private InputAction _leftTrackedAction;
    private InputAction _rightPositionAction;
    private InputAction _rightRotationAction;
    private InputAction _rightTrackedAction;

    void Start()
    {
        Debug.Log("=== XR Controller Diagnostics Starting ===");
        
        // Try to auto-find controllers if not assigned
        if (leftController == null || rightController == null)
        {
            FindControllers();
        }

        // Setup manual input actions for diagnostics
        SetupInputActions();
        
        // Log all connected XR devices
        LogXRDevices();
    }

    void FindControllers()
    {
        // Try to find by name
        var leftGO = GameObject.Find("Left Controller");
        var rightGO = GameObject.Find("Right Controller");
        
        if (leftGO != null) leftController = leftGO.transform;
        if (rightGO != null) rightController = rightGO.transform;
        
        if (logToConsole)
        {
            Debug.Log($"Auto-found Left Controller: {leftController != null}");
            Debug.Log($"Auto-found Right Controller: {rightController != null}");
        }
    }

    void SetupInputActions()
    {
        // Create input actions directly to test if data is coming through
        _leftPositionAction = new InputAction("LeftPosition", binding: "<XRController>{LeftHand}/devicePosition");
        _leftRotationAction = new InputAction("LeftRotation", binding: "<XRController>{LeftHand}/deviceRotation");
        _leftTrackedAction = new InputAction("LeftTracked", binding: "<XRController>{LeftHand}/isTracked");
        
        _rightPositionAction = new InputAction("RightPosition", binding: "<XRController>{RightHand}/devicePosition");
        _rightRotationAction = new InputAction("RightRotation", binding: "<XRController>{RightHand}/deviceRotation");
        _rightTrackedAction = new InputAction("RightTracked", binding: "<XRController>{RightHand}/isTracked");
        
        _leftPositionAction.Enable();
        _leftRotationAction.Enable();
        _leftTrackedAction.Enable();
        _rightPositionAction.Enable();
        _rightRotationAction.Enable();
        _rightTrackedAction.Enable();
    }

    void LogXRDevices()
    {
        var devices = InputSystem.devices;
        Debug.Log($"=== Total Input Devices: {devices.Count} ===");
        
        foreach (var device in devices)
        {
            if (device is XRController || device is XRHMD || device.name.Contains("XR") || device.name.Contains("Controller"))
            {
                Debug.Log($"XR Device: {device.name} | Enabled: {device.enabled} | ID: {device.deviceId}");
                
                // List all controls
                foreach (var control in device.allControls)
                {
                    if (control.name.Contains("position") || control.name.Contains("rotation") || control.name.Contains("tracked"))
                    {
                        Debug.Log($"  - Control: {control.name} | Value: {control.ReadValueAsObject()}");
                    }
                }
            }
        }
    }

    void Update()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== XR Controller Diagnostics ===");
        sb.AppendLine($"Time: {Time.time:F2}");
        sb.AppendLine();

        // Check left controller
        sb.AppendLine("LEFT CONTROLLER:");
        if (leftController != null)
        {
            sb.AppendLine($"  Transform Pos: {leftController.position}");
            sb.AppendLine($"  Transform Rot: {leftController.rotation.eulerAngles}");
        }
        else
        {
            sb.AppendLine("  Transform: NOT FOUND");
        }

        var leftPos = _leftPositionAction.ReadValue<Vector3>();
        var leftRot = _leftRotationAction.ReadValue<Quaternion>();
        var leftTracked = _leftTrackedAction.ReadValue<float>() > 0.5f;
        sb.AppendLine($"  Input Pos: {leftPos}");
        sb.AppendLine($"  Input Rot: {leftRot.eulerAngles}");
        sb.AppendLine($"  Is Tracked: {leftTracked}");
        sb.AppendLine();

        // Check right controller
        sb.AppendLine("RIGHT CONTROLLER:");
        if (rightController != null)
        {
            sb.AppendLine($"  Transform Pos: {rightController.position}");
            sb.AppendLine($"  Transform Rot: {rightController.rotation.eulerAngles}");
        }
        else
        {
            sb.AppendLine("  Transform: NOT FOUND");
        }

        var rightPos = _rightPositionAction.ReadValue<Vector3>();
        var rightRot = _rightRotationAction.ReadValue<Quaternion>();
        var rightTracked = _rightTrackedAction.ReadValue<float>() > 0.5f;
        sb.AppendLine($"  Input Pos: {rightPos}");
        sb.AppendLine($"  Input Rot: {rightRot.eulerAngles}");
        sb.AppendLine($"  Is Tracked: {rightTracked}");
        sb.AppendLine();

        // Check HMD
        var hmd = InputSystem.GetDevice<XRHMD>();
        if (hmd != null)
        {
            sb.AppendLine("HMD:");
            sb.AppendLine($"  Device: {hmd.name}");
            sb.AppendLine($"  Center Eye Pos: {hmd.centerEyePosition.ReadValue()}");
        }

        _debugInfo = sb.ToString();

        if (logToConsole && Time.frameCount % 60 == 0) // Log every 60 frames
        {
            Debug.Log(_debugInfo);
        }
    }

    void OnGUI()
    {
        if (!showDebugGUI) return;
        
        GUI.Box(new Rect(10, 10, 400, 300), "");
        GUI.Label(new Rect(20, 20, 380, 280), _debugInfo, new GUIStyle 
        { 
            fontSize = 12, 
            normal = new GUIStyleState { textColor = Color.white }
        });
    }

    void OnDestroy()
    {
        _leftPositionAction?.Disable();
        _leftRotationAction?.Disable();
        _leftTrackedAction?.Disable();
        _rightPositionAction?.Disable();
        _rightRotationAction?.Disable();
        _rightTrackedAction?.Disable();
    }
}
