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