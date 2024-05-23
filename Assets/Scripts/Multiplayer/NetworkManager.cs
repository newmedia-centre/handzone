using System;
using System.Collections.Generic;
using System.Linq;
using Schema.Socket.Unity;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    
    public GameObject playerPrefab;
    
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
        
        SessionClient.Instance.OnUnityPendant += OnUnityPendant;
        SessionClient.Instance.OnUnityPlayerData += UpdateUnityPlayers;
    }

    private void OnDestroy()
    {
        SessionClient.Instance.OnUnityPendant -= OnUnityPendant;
        SessionClient.Instance.OnUnityPlayerData -= UpdateUnityPlayers;
    }

    private void OnUnityPendant(UnityPendantIn obj)
    {
        Debug.Log("Pendant data received");
    }

    private void UpdateUnityPlayers(UnityPlayersOut incomingUnityPlayersData)
    {
        // Remove players that are not in the incoming data
        HashSet<string> playerIds = new HashSet<string>(incomingUnityPlayersData.Players.Select(x => x.Id));
        foreach (var player in _playerDictionary.Values)
        {
            if (!playerIds.Contains(player.Id))
            {
                RemovePlayer(player);
                _playerDictionary.Remove(player.Id);
            }
        }

        // Update existing players and add new players
        foreach (var incomingPlayerData in incomingUnityPlayersData.Players)
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

    public void AddPlayer(PlayerData obj)
    {
        if (!_playerDictionary.ContainsKey(obj.Id))
        {
            NetworkPlayer newPlayer = Instantiate(playerPrefab).GetComponent<NetworkPlayer>();
            newPlayer.Id = obj.Id;
            newPlayer.SetNameCard(obj.Name);
            newPlayer.SetColor(obj.Color);
            _playerDictionary.Add(newPlayer.Id, newPlayer);
        }
        else
        {
            Debug.LogWarning("Player with ID " + obj.Id + " already exists in the dictionary.");
        }
    }
    
    public void RemovePlayer(NetworkPlayer playerToRemove)
    {
        _playerDictionary.Remove(playerToRemove.Id);
        Destroy(playerToRemove.gameObject);
    }
}
