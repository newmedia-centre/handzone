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
using UnityEngine;

#endregion

/// <summary>
/// The PlayerInviteManager class handles the management of player invitations
/// in a multiplayer environment. It is responsible for creating and displaying
/// player invite UI elements, as well as managing the lifecycle of these invites.
/// The class listens for player invitation events from the session client and
/// updates the UI accordingly.
/// </summary>
public class PlayerInviteManager : MonoBehaviour
{
    public GameObject playerInvitePrefab;
    public Transform playerInvitesAnchor;

    private List<GameObject> playerInvites;

    /// <summary>
    /// Initializes the PlayerInviteManager by checking for required references
    /// and subscribing to player invitation events from the session client.
    /// </summary>
    private void Start()
    {
        if (playerInvitePrefab == null)
            Debug.LogError("Player invite prefab not set..");

        if (playerInvitesAnchor == null)
            Debug.LogWarning("Player invites anchor is not set, invites will not be parented.");

        if (SessionClient.Instance == null)
        {
            Debug.LogWarning(
                "SessionClient instance is null. Make sure to have a SessionClient instance in the scene.");
            return;
        }

        SessionClient.Instance.OnPlayerInvitation += CreatePlayerInvitation;
    }

    private void CreatePlayerInvitation(string playerId)
    {
        var go = Instantiate(playerInvitePrefab, playerInvitesAnchor);
        go.GetComponent<PlayerInvite>().playerNameText.text = playerId;
        playerInvites.Add(go);
    }
}