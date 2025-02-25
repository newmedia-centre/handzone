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

using System.Collections.Generic;
using Schema.Socket.Unity;
using TMPro;
using UnityEngine;

#endregion

/// <summary>
/// The NetworkPlayer class represents a player in the multiplayer environment.
/// It manages the player's data, including their ID, color, and name card display.
/// The class handles the synchronization of player state across the network,
/// including position, rotation, and interaction with other players and objects.
/// </summary>
public class NetworkPlayer : MonoBehaviour
{
    public string Id { get; set; }

    private PlayerData _receivedPlayerData;

    [Header("Client reference")] public GameObject clientHMD;

    [Header("Network Transforms")] public GameObject hmd;
    public GameObject left;
    public GameObject right;
    public NetworkVNCCursor cursor;

    [Header("Name Card")] public GameObject nameCard;
    public TMP_Text playerNameCard;

    [Header("Player material")] public Color color;
    public List<Renderer> playerRenderers;

    [Header("Interpolation Settings")] public float speed = 5.0F; // Movement speed in units per second.
    public float rotationSpeed = 8.0F; // Rotation speed in degrees per second.

    /// <summary>
    /// Initializes the player by setting the client HMD reference.
    /// </summary>
    private void Start()
    {
        // Receive the client HMD by looking for the main camera
        clientHMD = Camera.main.gameObject;
    }

    /// <summary>
    /// Updates the name card display with the player's name.
    /// </summary>
    /// <param name="playerName">The name to display on the name card.</param>
    public void SetNameCard(string playerName)
    {
        playerNameCard.SetText(playerName);
    }

    /// <summary>
    /// Sets the color of the player and updates the materials of the renderers.
    /// </summary>
    /// <param name="colorString">The color in HTML string format.</param>
    public void SetColor(string colorString)
    {
        color = ColorUtility.TryParseHtmlString(colorString, out var newColor) ? newColor : Color.white;

        foreach (var playerRenderer in playerRenderers) playerRenderer.material.color = color;
    }

    private void Update()
    {
        // This needs to be updated to make it rotate towards a client side
        var delta = nameCard.transform.position - clientHMD.transform.position;

        nameCard.transform.rotation = Quaternion.LookRotation(delta);

        if (_receivedPlayerData == null)
            return;


        // Define the speed of the interpolation
        var lerpSpeed = speed * Time.deltaTime;
        var rotationLerpSpeed = rotationSpeed * Time.deltaTime;

        // Define the target positions and rotations
        var hmdTargetPosition = new Vector3((float)_receivedPlayerData.Hmd.Position.X,
            (float)_receivedPlayerData.Hmd.Position.Y, (float)_receivedPlayerData.Hmd.Position.Z);
        var hmdTargetRotation = Quaternion.Euler(new Vector3((float)_receivedPlayerData.Hmd.Rotation.X,
            (float)_receivedPlayerData.Hmd.Rotation.Y, (float)_receivedPlayerData.Hmd.Rotation.Z));

        var leftTargetPosition = new Vector3((float)_receivedPlayerData.Left.Position.X,
            (float)_receivedPlayerData.Left.Position.Y, (float)_receivedPlayerData.Left.Position.Z);
        var leftTargetRotation = Quaternion.Euler(new Vector3((float)_receivedPlayerData.Left.Rotation.X,
            (float)_receivedPlayerData.Left.Rotation.Y, (float)_receivedPlayerData.Left.Rotation.Z));

        var rightTargetPosition = new Vector3((float)_receivedPlayerData.Right.Position.X,
            (float)_receivedPlayerData.Right.Position.Y, (float)_receivedPlayerData.Right.Position.Z);
        var rightTargetRotation = Quaternion.Euler(new Vector3((float)_receivedPlayerData.Right.Rotation.X,
            (float)_receivedPlayerData.Right.Rotation.Y, (float)_receivedPlayerData.Right.Rotation.Z));

        // Interpolate the position and rotation of the hmd, left, and right objects
        hmd.transform.position = Vector3.Lerp(hmd.transform.position, hmdTargetPosition, lerpSpeed);
        hmd.transform.rotation = Quaternion.Lerp(hmd.transform.rotation, hmdTargetRotation, rotationLerpSpeed);

        left.transform.position = Vector3.Lerp(left.transform.position, leftTargetPosition, lerpSpeed);
        left.transform.rotation = Quaternion.Lerp(left.transform.rotation, leftTargetRotation, rotationLerpSpeed);

        right.transform.position = Vector3.Lerp(right.transform.position, rightTargetPosition, lerpSpeed);
        right.transform.rotation = Quaternion.Lerp(right.transform.rotation, rightTargetRotation, rotationLerpSpeed);

        if (_receivedPlayerData.Cursor.X > 0.001f || _receivedPlayerData.Cursor.Y > 0.001f)
        {
            var cursorPosition = new Vector2((float)_receivedPlayerData.Cursor.X, (float)_receivedPlayerData.Cursor.Y);
            cursor.transform.localPosition = Vector3.Lerp(cursor.transform.localPosition, cursorPosition, lerpSpeed);
        }
    }

    public void UpdatePlayerData(PlayerData playerData)
    {
        _receivedPlayerData = playerData;
    }
}