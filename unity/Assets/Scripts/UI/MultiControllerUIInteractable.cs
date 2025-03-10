using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

/// <summary>
/// Handles interaction between multiple XR controllers and UI Toolkit interfaces in VR.
/// Attach this to the same GameObject as your UIDocument component.
/// </summary>
public class MultiControllerUIInteractable : XRBaseInteractable
{
    [Header("Visual Settings")]
    [SerializeField] private GameObject cursorPrefab;
    [SerializeField] private Color normalCursorColor = Color.white;
    [SerializeField] private Color hoverCursorColor = Color.yellow;
    [SerializeField] private Color pressCursorColor = Color.green;
    
    [Header("Interaction Settings")]
    [SerializeField] private float hapticIntensity = 0.5f;
    [SerializeField] private float hapticDuration = 0.1f;
    [SerializeField] private float positionSmoothTime = 0.05f;
    [SerializeField] private float controllerSwitchDelay = 0.5f; // Delay before switching active controller
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    private UIDocument uiDocument;
    private Dictionary<XRRayInteractor, InteractorData> interactorData = new Dictionary<XRRayInteractor, InteractorData>();
    private XRRayInteractor activeController; // The controller currently controlling the UI
    private float lastControllerSwitchTime; // Time when we last switched active controllers
    
    // Class to store per-interactor data
    private class InteractorData
    {
        public GameObject cursorInstance;
        public Vector3 cursorVelocity;
        public Vector3 targetPosition;
        public bool hasValidTarget;
        public float lastValidHitTime;
        public Vector2 lastValidPanelPosition;
        public bool isActive; // Whether this controller is currently active for UI interaction
    }
    
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
    
    private bool IsValidUV(Vector2 uv)
    {
        // Check if UV coordinates are within valid range (0-1)
        return uv.x >= 0f && uv.x <= 1f && uv.y >= 0f && uv.y <= 1f;
    }
    
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