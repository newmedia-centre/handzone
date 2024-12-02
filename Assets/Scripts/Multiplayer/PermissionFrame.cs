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
            Debug.LogWarning(
                "SessionClient instance is null. Make sure to have a SessionClient instance in the scene.");
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