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

using Schema.Socket.Unity;
using UnityEngine;

#endregion

/// <summary>
/// The PermissionFrame class is responsible for managing the visual representation
/// of permission status in a multiplayer environment. It modifies the sprite's
/// color based on the user's access rights to control a robot. The class listens
/// for updates from the session client to determine whether access is granted or denied,
/// and updates the frame's appearance accordingly.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PermissionFrame : MonoBehaviour
{
    public Color accessGrantedColor = Color.green;
    public Color accessDeniedColor = Color.red;

    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Initializes the PermissionFrame by obtaining the SpriteRenderer component
    /// and subscribing to session client events for permission updates.
    /// </summary>
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (SessionClient.Instance == null)
        {
            return;
        }

        SessionClient.Instance.OnUnityPendant += UpdatePermissionFrame;
    }

    /// <summary>
    /// Updates the color of the permission frame based on the user's ownership
    /// of the pendant.
    /// </summary>
    /// <param name="pendantOut">The data containing information about the pendant's owner.</param>
    private void UpdatePermissionFrame(UnityPendantOut pendantOut)
    {
        // Check if the current user is the owner of the pendant and change the color of the frame accordingly
        if (SessionClient.Instance.ClientId != pendantOut.Owner)
            _spriteRenderer.color = accessDeniedColor;
        else if (SessionClient.Instance.ClientId == pendantOut.Owner) _spriteRenderer.color = accessGrantedColor;
    }

    private void OnDestroy()
    {
        if (SessionClient.Instance)
            SessionClient.Instance.OnUnityPendant -= UpdatePermissionFrame;
    }
}