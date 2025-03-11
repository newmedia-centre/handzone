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

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

/// <summary>
/// Handles interaction between multiple XR controllers and UI Toolkit interfaces in VR.
/// This component allows XR ray interactors to interact with UI Toolkit UIs using a custom cursor
/// and panel space function. It supports switching between multiple controllers with visual feedback.
/// 
/// Key features:
/// - Visual cursor feedback for hovering and selection
/// - Haptic feedback on interactions
/// - Smooth cursor positioning and transitions
/// - Support for multiple controller interaction with priority system
/// - Custom panel space function for UI Toolkit integration
/// 
/// Attach this to the same GameObject as your UIDocument component.
/// </summary>
public class MultiControllerUIInteractable : XRBaseInteractable
{
    [Header("Visual Settings")]
    /// <summary>
    /// Prefab used for the visual cursor. Should contain a Renderer component.
    /// </summary>
    [SerializeField] private GameObject cursorPrefab;
    
    /// <summary>
    /// Color of the cursor when a controller is pointing at the UI but not the active controller.
    /// </summary>
    [SerializeField] private Color normalCursorColor = Color.white;
    
    /// <summary>
    /// Color of the cursor when a controller is the active one hovering over the UI.
    /// </summary>
    [SerializeField] private Color hoverCursorColor = Color.yellow;
    
    /// <summary>
    /// Color of the cursor when the controller is pressing/selecting the UI.
    /// </summary>
    [SerializeField] private Color pressCursorColor = Color.green;
    
    [Header("Interaction Settings")]
    /// <summary>
    /// Intensity of haptic feedback when interacting with UI elements (0-1).
    /// </summary>
    [SerializeField] private float hapticIntensity = 0.5f;
    
    /// <summary>
    /// Duration of haptic feedback in seconds when interacting with UI elements.
    /// </summary>
    [SerializeField] private float hapticDuration = 0.1f;
    
    /// <summary>
    /// Smoothing time for cursor position updates. Lower values make cursor movement more responsive but jittery.
    /// </summary>
    [SerializeField] private float positionSmoothTime = 0.05f;
    
    /// <summary>
    /// Delay before another controller can take over as the active controller (in seconds).
    /// Prevents rapid switching between controllers.
    /// </summary>
    [SerializeField] private float controllerSwitchDelay = 0.5f;
    
    [Header("Debug")]
    /// <summary>
    /// When enabled, outputs debug information to the console about controller interactions.
    /// </summary>
    [SerializeField] private bool showDebugInfo = false;
    
    /// <summary>
    /// Reference to the UIDocument component attached to this GameObject.
    /// </summary>
    private UIDocument uiDocument;
    
    /// <summary>
    /// Dictionary mapping each interactor to its associated data (cursor, state, etc.).
    /// </summary>
    private Dictionary<XRRayInteractor, InteractorData> interactorData = new Dictionary<XRRayInteractor, InteractorData>();
    
    /// <summary>
    /// The controller currently controlling the UI cursor and receiving UI events.
    /// </summary>
    private XRRayInteractor activeController;
    
    /// <summary>
    /// Time when we last switched active controllers, used for the switch delay.
    /// </summary>
    private float lastControllerSwitchTime;
    
    /// <summary>
    /// Class to store per-interactor data including cursor information and interaction state.
    /// </summary>
    private class InteractorData
    {
        /// <summary>Instance of the cursor GameObject for this interactor.</summary>
        public GameObject cursorInstance;
        
        /// <summary>Velocity vector used for smooth damping of cursor position.</summary>
        public Vector3 cursorVelocity;
        
        /// <summary>Target position for the cursor based on ray hit.</summary>
        public Vector3 targetPosition;
        
        /// <summary>Whether this interactor has a valid UI target.</summary>
        public bool hasValidTarget;
        
        /// <summary>Time when this interactor last had a valid hit.</summary>
        public float lastValidHitTime;
        
        /// <summary>The last valid panel position in UI coordinates.</summary>
        public Vector2 lastValidPanelPosition;
        
        /// <summary>Whether this controller is currently active for UI interaction.</summary>
        public bool isActive;
    }
    
    /// <summary>
    /// Initializes component references and validates required components.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component is required on the same GameObject");
            enabled = false;
            return;
        }
    }

    /// <summary>
    /// Sets up event listeners and configures the UI Document's panel space function when enabled.
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (uiDocument != null)
        {
            // Set up the custom screen-to-panel space function
            uiDocument.panelSettings.SetScreenToPanelSpaceFunction(CustomScreenToPanelSpace);
            
            if (showDebugInfo)
            {
                Debug.Log($"UI Document enabled with custom panel space function");
            }
        }

        hoverEntered.AddListener(HandleHoverEnter);
        hoverExited.AddListener(HandleHoverExit);
        selectEntered.AddListener(HandleSelectEnter);
        selectExited.AddListener(HandleSelectExit);
        
        lastControllerSwitchTime = 0f;
    }

    /// <summary>
    /// Cleans up event listeners and destroys cursor instances when disabled.
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        
        // Clean up all cursors
        foreach (var data in interactorData.Values)
        {
            if (data.cursorInstance != null)
            {
                Destroy(data.cursorInstance);
            }
        }
        interactorData.Clear();
        activeController = null;

        hoverEntered.RemoveListener(HandleHoverEnter);
        hoverExited.RemoveListener(HandleHoverExit);
        selectEntered.RemoveListener(HandleSelectEnter);
        selectExited.RemoveListener(HandleSelectExit);
    }

    /// <summary>
    /// Handles the hover enter event for an interactor.
    /// Creates cursor instances for new interactors and may set them as the active controller.
    /// </summary>
    /// <param name="args">Event arguments containing the interactor information.</param>
    private void HandleHoverEnter(HoverEnterEventArgs args)
    {
        if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            // Create interactor data if needed
            if (!interactorData.ContainsKey(rayInteractor))
            {
                var data = new InteractorData
                {
                    cursorVelocity = Vector3.zero,
                    hasValidTarget = false,
                    lastValidHitTime = 0f,
                    isActive = false
                };
                
                // Create cursor if prefab is provided
                if (cursorPrefab != null)
                {
                    data.cursorInstance = Instantiate(cursorPrefab);
                    data.cursorInstance.name = $"UI Cursor ({rayInteractor.name})";
                    
                    // Set initial color
                    var renderer = data.cursorInstance.GetComponentInChildren<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = normalCursorColor;
                    }
                }
                
                interactorData[rayInteractor] = data;
            }
            
            // If no active controller or enough time has passed since last switch, make this the active one
            if (activeController == null || 
                (Time.time - lastControllerSwitchTime > controllerSwitchDelay && 
                 rayInteractor.isSelectActive)) // Only switch if selecting
            {
                SetActiveController(rayInteractor);
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"Hover enter from {rayInteractor.name}, Active: {(activeController == rayInteractor)}");
            }
        }
    }

    /// <summary>
    /// Handles the hover exit event for an interactor.
    /// Hides the cursor and may select a new active controller if the current one is exiting.
    /// </summary>
    /// <param name="args">Event arguments containing the interactor information.</param>
    private void HandleHoverExit(HoverExitEventArgs args)
    {
        if (args.interactorObject is XRRayInteractor rayInteractor && interactorData.TryGetValue(rayInteractor, out var data))
        {
            // Hide cursor
            if (data.cursorInstance != null)
            {
                data.cursorInstance.SetActive(false);
            }
            
            data.hasValidTarget = false;
            
            // If this was the active controller, find a new one
            if (rayInteractor == activeController)
            {
                activeController = null;
                
                // Find another controller with a valid target
                foreach (var kvp in interactorData)
                {
                    if (kvp.Key != rayInteractor && kvp.Value.hasValidTarget)
                    {
                        SetActiveController(kvp.Key);
                        break;
                    }
                }
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"Hover exit from {rayInteractor.name}, New active: {(activeController?.name ?? "None")}");
            }
        }
    }

    /// <summary>
    /// Handles the select enter event for an interactor (when trigger is pressed).
    /// Sets the controller as active, updates cursor color, and sends haptic feedback.
    /// </summary>
    /// <param name="args">Event arguments containing the interactor information.</param>
    private void HandleSelectEnter(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRRayInteractor rayInteractor && interactorData.TryGetValue(rayInteractor, out var data))
        {
            // Make this the active controller immediately when selecting
            SetActiveController(rayInteractor);
            
            // Update cursor color
            if (data.cursorInstance != null)
            {
                var renderer = data.cursorInstance.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = pressCursorColor;
                }
            }
            
            // Send haptic feedback
            SendHapticFeedback(rayInteractor, hapticIntensity, hapticDuration);
            
            if (showDebugInfo)
            {
                Debug.Log($"Select enter from {rayInteractor.name}, Now active");
            }
        }
    }

    /// <summary>
    /// Handles the select exit event for an interactor (when trigger is released).
    /// Updates cursor color and sends haptic feedback.
    /// </summary>
    /// <param name="args">Event arguments containing the interactor information.</param>
    private void HandleSelectExit(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRRayInteractor rayInteractor && interactorData.TryGetValue(rayInteractor, out var data))
        {
            // Update cursor color
            if (data.cursorInstance != null)
            {
                var renderer = data.cursorInstance.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = data.isActive ? hoverCursorColor : normalCursorColor;
                }
            }
            
            // Send haptic feedback
            SendHapticFeedback(rayInteractor, hapticIntensity * 0.7f, hapticDuration * 0.7f);
            
            if (showDebugInfo)
            {
                Debug.Log($"Select exit from {rayInteractor.name}");
            }
        }
    }

    /// <summary>
    /// Sets the specified controller as the active one for UI interaction.
    /// Updates cursor colors for both previous and new active controllers.
    /// </summary>
    /// <param name="controller">The controller to set as active.</param>
    private void SetActiveController(XRRayInteractor controller)
    {
        if (activeController == controller)
            return;
            
        // Deactivate the previous controller
        if (activeController != null && interactorData.TryGetValue(activeController, out var prevData))
        {
            prevData.isActive = false;
            
            // Update cursor color
            if (prevData.cursorInstance != null)
            {
                var renderer = prevData.cursorInstance.GetComponentInChildren<Renderer>();
                if (renderer != null && !activeController.isSelectActive)
                {
                    renderer.material.color = normalCursorColor;
                }
            }
        }
        
        // Set the new active controller
        activeController = controller;
        lastControllerSwitchTime = Time.time;
        
        // Activate the new controller
        if (controller != null && interactorData.TryGetValue(controller, out var newData))
        {
            newData.isActive = true;
            
            // Update cursor color
            if (newData.cursorInstance != null)
            {
                var renderer = newData.cursorInstance.GetComponentInChildren<Renderer>();
                if (renderer != null && !controller.isSelectActive)
                {
                    renderer.material.color = hoverCursorColor;
                }
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"Active controller changed to: {controller?.name ?? "None"}");
        }
    }

    /// <summary>
    /// Updates the state of all interactors and their cursors every frame.
    /// Removes any destroyed interactors from tracking.
    /// </summary>
    private void Update()
    {
        // Process all interactors
        List<XRRayInteractor> interactorsToRemove = new List<XRRayInteractor>();
        
        foreach (var kvp in interactorData)
        {
            var rayInteractor = kvp.Key;
            var data = kvp.Value;
            
            if (rayInteractor == null || !rayInteractor.isActiveAndEnabled)
            {
                interactorsToRemove.Add(rayInteractor);
                continue;
            }
            
            ProcessInteractor(rayInteractor, data);
        }
        
        // Clean up any destroyed interactors
        foreach (var interactor in interactorsToRemove)
        {
            if (interactorData.TryGetValue(interactor, out var data) && data.cursorInstance != null)
            {
                Destroy(data.cursorInstance);
            }
            
            interactorData.Remove(interactor);
            
            if (activeController == interactor)
            {
                activeController = null;
            }
        }
    }
    
    /// <summary>
    /// Processes a single interactor's state, updating its cursor position, visibility, and active state.
    /// Also handles raycast hit detection and UI position updates.
    /// </summary>
    /// <param name="rayInteractor">The ray interactor to process.</param>
    /// <param name="data">The associated data for this interactor.</param>
    private void ProcessInteractor(XRRayInteractor rayInteractor, InteractorData data)
    {
        bool validHitThisFrame = false;
        
        if (rayInteractor.TryGetCurrent3DRaycastHit(out var hit))
        {
            // Only process hits on this gameObject
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                validHitThisFrame = true;
                data.lastValidHitTime = Time.time;
                data.targetPosition = hit.point;
                data.hasValidTarget = true;
                
                // If this is the active controller, update the panel position
                if (rayInteractor == activeController)
                {
                    // Convert texture coordinates to panel space for validation
                    var pixelUV = hit.textureCoord;
                    
                    // Validate the UV coordinates
                    if (IsValidUV(pixelUV))
                    {
                        pixelUV.y = 1 - pixelUV.y; // Flip Y coordinate
                        
                        // Scale to panel dimensions
                        pixelUV.x *= uiDocument.panelSettings.targetTexture.width;
                        pixelUV.y *= uiDocument.panelSettings.targetTexture.height;
                        
                        data.lastValidPanelPosition = pixelUV;
                    }
                }
            }
        }
        
        // Handle cursor visibility and position
        if (data.cursorInstance != null)
        {
            // Only show cursor if we have a valid target or had one recently
            bool shouldShowCursor = data.hasValidTarget && 
                                   (validHitThisFrame || Time.time - data.lastValidHitTime < 0.1f);
            
            data.cursorInstance.SetActive(shouldShowCursor);
            
            if (shouldShowCursor)
            {
                // Smooth cursor position
                data.cursorInstance.transform.position = Vector3.SmoothDamp(
                    data.cursorInstance.transform.position, 
                    data.targetPosition, 
                    ref data.cursorVelocity, 
                    positionSmoothTime
                );
                
                // Orient cursor to face the normal
                if (validHitThisFrame)
                {
                    data.cursorInstance.transform.rotation = Quaternion.LookRotation(-hit.normal);
                }
                
                // Update cursor color based on interaction state
                var renderer = data.cursorInstance.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    if (rayInteractor.isSelectActive)
                    {
                        renderer.material.color = pressCursorColor;
                    }
                    else if (rayInteractor == activeController)
                    {
                        renderer.material.color = hoverCursorColor;
                    }
                    else
                    {
                        renderer.material.color = normalCursorColor;
                    }
                }
            }
        }
        
        // If this interactor is no longer valid and it's the active one, try to find a new active controller
        if (!validHitThisFrame && rayInteractor == activeController && Time.time - data.lastValidHitTime > 0.2f)
        {
            // Find another controller with a valid target
            XRRayInteractor newActive = null;
            
            foreach (var kvp in interactorData)
            {
                if (kvp.Key != rayInteractor && kvp.Value.hasValidTarget)
                {
                    newActive = kvp.Key;
                    break;
                }
            }
            
            if (newActive != null)
            {
                SetActiveController(newActive);
            }
            else
            {
                activeController = null;
            }
        }
    }
    
    /// <summary>
    /// Custom function that converts screen position to panel space for UI Toolkit.
    /// This is the key integration point with UI Toolkit, providing cursor position from the ray.
    /// </summary>
    /// <param name="screenPosition">The screen position to convert (not used, as we get position from ray).</param>
    /// <returns>The panel space position for UI Toolkit, or an off-screen position if invalid.</returns>
    private Vector2 CustomScreenToPanelSpace(Vector2 screenPosition)
    {
        var invalidPosition = new Vector2(-10000, -10000); // Far off-screen
        
        // If no active controller, return invalid position
        if (activeController == null || !interactorData.TryGetValue(activeController, out var data))
        {
            return invalidPosition;
        }
        
        // If the active controller has a valid panel position, return it
        if (data.hasValidTarget)
        {
            return data.lastValidPanelPosition;
        }
        
        return invalidPosition;
    }
    
    /// <summary>
    /// Checks if UV coordinates are within the valid 0-1 range.
    /// </summary>
    /// <param name="uv">The UV coordinates to validate.</param>
    /// <returns>True if the coordinates are within valid range, false otherwise.</returns>
    private bool IsValidUV(Vector2 uv)
    {
        // Check if UV coordinates are within valid range (0-1)
        return uv.x >= 0f && uv.x <= 1f && uv.y >= 0f && uv.y <= 1f;
    }
    
    /// <summary>
    /// Sends haptic feedback to a ray interactor.
    /// Supports XR Interaction Toolkit 3.0+ with HapticImpulsePlayer component.
    /// </summary>
    /// <param name="rayInteractor">The ray interactor to send haptic feedback to.</param>
    /// <param name="intensity">The intensity of the haptic feedback (0-1).</param>
    /// <param name="duration">The duration of the haptic feedback in seconds.</param>
    private void SendHapticFeedback(XRRayInteractor rayInteractor, float intensity, float duration)
    {
        // In XR Interaction Toolkit 3.0+, haptic feedback is handled through the HapticImpulsePlayer
        if (rayInteractor.TryGetComponent<HapticImpulsePlayer>(out var hapticPlayer))
        {
            hapticPlayer.SendHapticImpulse(intensity, duration);
        }
        else
        {
            // Try to find the HapticImpulsePlayer in children
            hapticPlayer = rayInteractor.GetComponentInChildren<HapticImpulsePlayer>();
            if (hapticPlayer != null)
            {
                hapticPlayer.SendHapticImpulse(intensity, duration);
            }
            else if (showDebugInfo)
            {
                Debug.LogWarning($"No HapticImpulsePlayer found on {rayInteractor.name}. Add one to enable haptic feedback.");
            }
        }
    }
}