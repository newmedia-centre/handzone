using System.Collections.Generic;
using System.Linq;
using Schema.Socket.Unity;
using UnityEngine;

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
    }
    
    private void Start()
    {
        if(SessionClient.Instance == null)
        {
            Debug.LogWarning("SessionClient instance is null. Make sure to have a SessionClient instance in the scene.");
            return;
        }
        
        SessionClient.Instance.OnUnityPlayerData += UpdateNetworkPlayers;
        SessionClient.Instance.OnUnityPendant += UpdateNetworkPendant;
        
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
        SessionClient.Instance.OnUnityPlayerData -= UpdateNetworkPlayers;
        SessionClient.Instance.OnUnityPendant -= UpdateNetworkPendant;
        
        ClearPlayers();
    }

    private void UpdateNetworkPendant(UnityPendantOut incomingPendantData)
    {
        
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

        // Update existing players and add new players when necessary
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
            
            // Create cursor for the player
            
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
