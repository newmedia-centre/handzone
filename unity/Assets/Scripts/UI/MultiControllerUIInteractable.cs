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
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    private UIDocument uiDocument;
    private List<XRRayInteractor> activeInteractors = new List<XRRayInteractor>();
    private Dictionary<XRRayInteractor, InteractorData> interactorData = new Dictionary<XRRayInteractor, InteractorData>();
    private XRRayInteractor currentInteractor;
    private Vector2 lastValidPanelPosition = Vector2.zero;
    private bool hasValidPosition = false;
    
    // Class to store per-interactor data
    private class InteractorData
    {
        public GameObject cursorInstance;
        public Vector3 cursorVelocity;
        public Vector3 targetPosition;
        public bool hasValidTarget;
        public float lastValidHitTime;
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
        activeInteractors.Clear();
        currentInteractor = null;
        hasValidPosition = false;

        hoverEntered.RemoveListener(HandleHoverEnter);
        hoverExited.RemoveListener(HandleHoverExit);
        selectEntered.RemoveListener(HandleSelectEnter);
        selectExited.RemoveListener(HandleSelectExit);
    }

    private void HandleHoverEnter(HoverEnterEventArgs args)
    {
        if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            if (!activeInteractors.Contains(rayInteractor))
            {
                activeInteractors.Add(rayInteractor);
                
                // Create interactor data if needed
                if (!interactorData.ContainsKey(rayInteractor))
                {
                    var data = new InteractorData
                    {
                        cursorVelocity = Vector3.zero,
                        hasValidTarget = false,
                        lastValidHitTime = 0f
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
            }
            
            // Set as current interactor
            currentInteractor = rayInteractor;
            
            if (showDebugInfo)
            {
                Debug.Log($"Hover enter from {rayInteractor.name}");
            }
        }
    }

    private void HandleHoverExit(HoverExitEventArgs args)
    {
        if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            activeInteractors.Remove(rayInteractor);
            
            // Hide cursor
            if (interactorData.TryGetValue(rayInteractor, out var data) && data.cursorInstance != null)
            {
                data.cursorInstance.SetActive(false);
                data.hasValidTarget = false;
            }
            
            // Update current interactor
            if (currentInteractor == rayInteractor)
            {
                currentInteractor = activeInteractors.Count > 0 ? activeInteractors[0] : null;
                
                // If we lost our current interactor, we need to invalidate the position
                if (currentInteractor == null)
                {
                    hasValidPosition = false;
                }
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"Hover exit from {rayInteractor.name}");
            }
        }
    }

    private void HandleSelectEnter(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            // Set as current interactor
            currentInteractor = rayInteractor;
            
            // Update cursor color
            if (interactorData.TryGetValue(rayInteractor, out var data) && data.cursorInstance != null)
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
                Debug.Log($"Select enter from {rayInteractor.name}");
            }
        }
    }

    private void HandleSelectExit(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            // Update cursor color
            if (interactorData.TryGetValue(rayInteractor, out var data) && data.cursorInstance != null)
            {
                var renderer = data.cursorInstance.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = hoverCursorColor;
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

    private void Update()
    {
        // Update all active interactors and their cursors
        foreach (var rayInteractor in activeInteractors.ToArray()) // Use ToArray to avoid collection modified exception
        {
            if (rayInteractor == null || !rayInteractor.isActiveAndEnabled)
            {
                activeInteractors.Remove(rayInteractor);
                continue;
            }
            
            if (!interactorData.TryGetValue(rayInteractor, out var data))
                continue;
            
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
                    
                    // If this is the current interactor, update the panel position
                    if (rayInteractor == currentInteractor)
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
                            
                            lastValidPanelPosition = pixelUV;
                            hasValidPosition = true;
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
                        else if (rayInteractor == currentInteractor)
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
            
            // If this interactor is no longer valid and it's the current one, try to find a new current
            if (!validHitThisFrame && rayInteractor == currentInteractor && Time.time - data.lastValidHitTime > 0.2f)
            {
                // Find another valid interactor
                currentInteractor = null;
                foreach (var otherInteractor in activeInteractors)
                {
                    if (otherInteractor != rayInteractor && 
                        interactorData.TryGetValue(otherInteractor, out var otherData) && 
                        otherData.hasValidTarget)
                    {
                        currentInteractor = otherInteractor;
                        break;
                    }
                }
                
                // If we couldn't find another valid interactor, invalidate the position
                if (currentInteractor == null)
                {
                    hasValidPosition = false;
                }
            }
        }
    }
    
    private Vector2 CustomScreenToPanelSpace(Vector2 screenPosition)
    {
        var invalidPosition = new Vector2(float.NaN, float.NaN);
        
        // If we don't have a valid position, return invalid
        if (!hasValidPosition || currentInteractor == null)
        {
            return invalidPosition;
        }
        
        // Return the last valid panel position
        return lastValidPanelPosition;
    }
    
    private bool IsValidUV(Vector2 uv)
    {
        // Check if UV coordinates are within valid range (0-1)
        return uv.x >= 0f && uv.x <= 1f && uv.y >= 0f && uv.y <= 1f;
    }
    
    private void SendHapticFeedback(XRRayInteractor rayInteractor, float intensity, float duration)
    {
        // In XR Interaction Toolkit 3.0+, haptic feedback is handled through the HapticImpulsePlayer
        // Try to get the HapticImpulsePlayer component from the interactor's GameObject
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