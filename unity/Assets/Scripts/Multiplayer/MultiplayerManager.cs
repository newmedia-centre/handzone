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
using System.Linq;
using Schema.Socket.Unity;
using UnityEngine;

#endregion

/// <summary>
/// The MultiplayerManager class is responsible for managing multiplayer sessions
/// in the application. It handles the instantiation and management of networked
/// player objects, updates player data based on incoming network messages, and
/// ensures that there is only one instance of the MultiplayerManager in the scene.
/// The class also manages the lifecycle of players, including adding and removing
/// players from the game based on network events.
/// </summary>
public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance { get; private set; }
    public GameObject networkPlayerPrefab;
    public GameObject playerCursorPrefab;
    public Transform cursorAnchor;

    private Dictionary<string, NetworkPlayer> _playerDictionary = new();
    private UnityPlayersOut _previousPlayersData;

    /// <summary>
    /// Initializes the MultiplayerManager instance and ensures that only one
    /// instance exists in the scene. It also subscribes to network events
    /// for player data updates.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple instances of NetworkManager detected. Destroying the duplicate instance.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Starts the MultiplayerManager and subscribes to the SessionClient's
    /// player data update events.
    /// </summary>
    private void Start()
    {
        if (SessionClient.Instance == null)
        {
            Debug.LogWarning(
                "SessionClient instance is null. Make sure to have a SessionClient instance in the scene.");
            return;
        }

        SessionClient.Instance.OnUnityPlayerData += UpdateNetworkPlayers;
    }

    /// <summary>
    /// Clears all players from the multiplayer session, destroying their
    /// associated game objects and clearing the player dictionary.
    /// </summary>
    private void ClearPlayers()
    {
        foreach (var player in _playerDictionary.Values.Where(player => player != null)) Destroy(player.gameObject);
        _playerDictionary.Clear();
    }

    /// <summary>
    /// Cleans up the MultiplayerManager instance when it is destroyed,
    /// unsubscribing from network events and clearing player data.
    /// </summary>
    private void OnDestroy()
    {
        if (SessionClient.Instance)
            SessionClient.Instance.OnUnityPlayerData -= UpdateNetworkPlayers;

        ClearPlayers();
    }

    /// <summary>
    /// Updates the network players based on incoming player data from the server.
    /// This includes adding new players, updating existing players, and removing
    /// players that are no longer present.
    /// </summary>
    /// <param name="incomingPlayersData">The incoming player data from the server.</param>
    private void UpdateNetworkPlayers(UnityPlayersOut incomingPlayersData)
    {
        // Remove the local client from the incoming data
        incomingPlayersData.Players.RemoveAll(x => x.Id == SessionClient.Instance.ClientId);

        // Find players that are not in the incoming data and remove them
        if (_previousPlayersData != null)
        {
            var playersToRemove = _previousPlayersData.Players
                .Where(x => !incomingPlayersData.Players.Select(y => y.Id).Contains(x.Id)).ToList();
            foreach (var player in playersToRemove)
                if (_playerDictionary.TryGetValue(player.Id, out var playerToRemove))
                    RemovePlayer(playerToRemove);
        }

        // Update existing players and add new players when necessary
        foreach (var incomingPlayerData in incomingPlayersData.Players)
            if (_playerDictionary.TryGetValue(incomingPlayerData.Id, out var player))
            {
                player.UpdatePlayerData(incomingPlayerData);
            }
            else
            {
                AddPlayer(incomingPlayerData);
                Debug.Log(incomingPlayerData.Id + " is not in the player dictionary.");
                Debug.Log("Client ID: " + SessionClient.Instance.ClientId);
            }
    }

    /// <summary>
    /// Adds a new player to the multiplayer session based on the provided player data.
    /// </summary>
    /// <param name="playerData">The data of the player to be added.</param>
    public void AddPlayer(PlayerData playerData)
    {
        if (!_playerDictionary.ContainsKey(playerData.Id))
        {
            // Create a new player instance
            var newPlayer = Instantiate(networkPlayerPrefab).GetComponent<NetworkPlayer>();
            newPlayer.Id = playerData.Id;
            newPlayer.SetNameCard(playerData.Name);
            newPlayer.SetColor(playerData.Color);

            // Create cursor for the player
            var cursor = Instantiate(playerCursorPrefab, cursorAnchor).GetComponent<NetworkVNCCursor>();
            cursor.Color = newPlayer.color;
            cursor.PlayerNameLabel = playerData.Name;
            newPlayer.cursor = cursor;

            _playerDictionary.Add(newPlayer.Id, newPlayer);
        }
    }

    /// <summary>
    /// Removes a player from the multiplayer session.
    /// </summary>
    /// <param name="playerToRemove">The player to be removed.</param>
    public void RemovePlayer(NetworkPlayer playerToRemove)
    {
        _playerDictionary.Remove(playerToRemove.Id);
        Destroy(playerToRemove.gameObject);
    }
}