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

using System.Collections.Generic;
using Schema.Socket.Unity;
using UnityEngine;
using VNCScreen;

#endregion

/// <summary>
/// The LocalPlayer class manages the local player's state and interactions
/// in a multiplayer environment. It handles the player's input from VR
/// devices, sends player data to the server, and manages the active cursor
/// for interactions with the virtual environment. The class ensures that
/// only one instance of the LocalPlayer exists in the scene, implementing
/// the singleton pattern.
/// </summary>
public class LocalPlayer : MonoBehaviour
{
    public Transform hmdRef;
    public Transform leftControllerRef;
    public Transform rightControllerRef;
    public List<VNCXRRaycaster> cursorRefs;

    private VNCXRRaycaster _activeCursor;
    private float _sendInterval = 0.1f;
    private float _sendTimer = 0.0f;

    public static LocalPlayer Instance { get; private set; }

    /// <summary>
    /// Initializes the LocalPlayer instance and checks for the presence of
    /// required references. It also ensures that only one instance of
    /// LocalPlayer exists in the scene.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple instances of LocalPlayer detected. Destroying the duplicate instance.");
            Destroy(gameObject);
        }

        if (hmdRef == null) Debug.LogError("HMD reference not set.");
        if (leftControllerRef == null) Debug.LogError("Left controller reference not set.");
        if (rightControllerRef == null) Debug.LogError("Right controller reference not set.");
    }

    /// <summary>
    /// Cleans up the LocalPlayer instance when it is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// Sends the local player's data to the server at regular intervals.
    /// </summary>
    /// <param name="playerIn">The UnityPlayerIn object containing player data.</param>
    public void SendPlayerData(UnityPlayerIn playerIn)
    {
        if (SessionClient.Instance && SessionClient.Instance.IsConnected)
        {
            // Send the player data to the server with a time interval
            if (_sendTimer < _sendInterval)
            {
                _sendTimer += Time.deltaTime;
                return;
            }

            SessionClient.Instance.SendUnityPlayerIn(playerIn);
            _sendTimer = 0.0f;
        }
    }

    /// <summary>
    /// Updates the local player's state and sends data to the server.
    /// This method is called once per frame.
    /// </summary>
    private void Update()
    {
        foreach (var cursor in cursorRefs)
            // Only send the cursor data of any cursor is hitting a VNCScreen if possible
            if (cursor.IsHitting)
            {
                _activeCursor = cursor;
                break;
            }

        // If no cursor is hitting a VNCScreen, use the first cursor in the list
        if (!_activeCursor) _activeCursor = cursorRefs[0];

        // Create new player data to send to the server
        var playerIn = new UnityPlayerIn
        {
            Hmd = Utility.TransformToSixDofPosition(hmdRef.transform),
            Left = Utility.TransformToSixDofPosition(leftControllerRef.transform),
            Right = Utility.TransformToSixDofPosition(rightControllerRef.transform),
            Cursor = _activeCursor.TextureCoord
        };

        // Send local player data to the server
        SendPlayerData(playerIn);
    }
}