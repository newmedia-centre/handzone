using System.Collections.Generic;
using System.Linq;
using Schema.Socket.Unity;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance { get; private set; }
    public GameObject networkPlayerPrefab;
    public GameObject playerCursorPrefab;
    public Transform cursorAnchor;

    private Dictionary<string, NetworkPlayer> _playerDictionary = new();
    private UnityPlayersOut _previousPlayersData;

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
    }
    
    private void ClearPlayers()
    {
        foreach (var player in _playerDictionary.Values.Where(player => player != null))
        {
            Destroy(player.gameObject);
        }
        _playerDictionary.Clear();
    }
    
    private void OnDestroy()
    {
        if(SessionClient.Instance)
            SessionClient.Instance.OnUnityPlayerData -= UpdateNetworkPlayers;
        
        ClearPlayers();
    }
    
    private void UpdateNetworkPlayers(UnityPlayersOut incomingPlayersData)
    {
        
        // Remove the local client from the incoming data
        incomingPlayersData.Players.RemoveAll(x => x.Id == SessionClient.Instance.ClientId);
        
        // Find players that are not in the incoming data and remove them
        if (_previousPlayersData != null)
        {
            var playersToRemove = _previousPlayersData.Players.Where(x => !incomingPlayersData.Players.Select(y => y.Id).Contains(x.Id)).ToList();
            foreach (var player in playersToRemove)
            {
                if (_playerDictionary.TryGetValue(player.Id, out var playerToRemove))
                {
                    RemovePlayer(playerToRemove);
                }
            }
        }
        
        // Update existing players and add new players when necessary
        foreach (var incomingPlayerData in incomingPlayersData.Players)
        {
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
    }

    public void AddPlayer(PlayerData playerData)
    {
        if (!_playerDictionary.ContainsKey(playerData.Id))
        {
            // Create a new player instance
            NetworkPlayer newPlayer = Instantiate(networkPlayerPrefab).GetComponent<NetworkPlayer>();
            newPlayer.Id = playerData.Id;
            newPlayer.SetNameCard(playerData.Name);
            newPlayer.SetColor(playerData.Color);
            
            // Create cursor for the player
            NetworkVNCCursor cursor = Instantiate(playerCursorPrefab, cursorAnchor).GetComponent<NetworkVNCCursor>();
            cursor.Color = newPlayer.color;
            cursor.PlayerNameLabel = playerData.Name;
            newPlayer.cursor = cursor;
            
            _playerDictionary.Add(newPlayer.Id, newPlayer);
        }
    }
    
    public void RemovePlayer(NetworkPlayer playerToRemove)
    {
        _playerDictionary.Remove(playerToRemove.Id);
        Destroy(playerToRemove.gameObject);
    }
}
