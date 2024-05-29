using System;
using System.Collections.Generic;
using System.Linq;
using Schema.Socket.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance { get; private set; }
    
    public GameObject networkPlayerPrefab;
    
    private Dictionary<string, NetworkPlayer> _playerDictionary = new();

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
        
        SessionClient.Instance.OnUnityPendant += UpdatePendant;
        SessionClient.Instance.OnUnityPlayerData += UpdateNetworkPlayers;
        
        // Clear the player dictionary
        ClearPlayers();
    }

    private void ClearPlayers()
    {
        foreach (var player in _playerDictionary.Values)
        {
            Destroy(player.gameObject);
        }
        _playerDictionary.Clear();
    }
    
    private void OnDestroy()
    {
        SessionClient.Instance.OnUnityPendant -= UpdatePendant;
        SessionClient.Instance.OnUnityPlayerData -= UpdateNetworkPlayers;
        
        ClearPlayers();
    }

    private void UpdatePendant(UnityPendantIn obj)
    {
        Debug.Log("Pendant data received");
    }

    private void UpdateNetworkPlayers(UnityPlayersOut incomingPlayersData)
    {
        // Remove players that are not in the incoming data
        HashSet<string> playerIds = new HashSet<string>(incomingPlayersData.Players.Select(x => x.Id));
        
        // Also filter the local player from the dictionary
        if(incomingPlayersData.Players.All(x => x.Id != SessionClient.Instance.ClientId))
        {
            playerIds.Add(SessionClient.Instance.ClientId);
        }
        
        foreach (var player in _playerDictionary.Values)
        {
            if (!playerIds.Contains(player.Id))
            {
                RemovePlayer(player);
                _playerDictionary.Remove(player.Id);
            }
        }

        // Update existing players and add new players
        foreach (var incomingPlayerData in incomingPlayersData.Players)
        {
            if (_playerDictionary.TryGetValue(incomingPlayerData.Id, out var player))
            {
                player.UpdatePositions(incomingPlayerData);
            }
            else
            {
                AddPlayer(incomingPlayerData);
            }
        }
    }

    public void AddPlayer(PlayerData playerData)
    {
        if (!_playerDictionary.ContainsKey(playerData.Id))
        {
            NetworkPlayer newPlayer = Instantiate(networkPlayerPrefab).GetComponent<NetworkPlayer>();
            newPlayer.Id = playerData.Id;
            newPlayer.SetNameCard(playerData.Name);
            newPlayer.SetColor(playerData.Color);
            _playerDictionary.Add(newPlayer.Id, newPlayer);
        }
        else
        {
            Debug.LogWarning("Player with ID " + playerData.Id + " already exists in the dictionary.");
        }
    }
    
    public void RemovePlayer(NetworkPlayer playerToRemove)
    {
        _playerDictionary.Remove(playerToRemove.Id);
        Destroy(playerToRemove.gameObject);
    }
}
