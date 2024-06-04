using System.Collections.Generic;
using System.Linq;
using Schema.Socket.Unity;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance { get; private set; }
    public GameObject networkPlayerPrefab;
    public GameObject playerCursorPrefab;
    
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
        foreach (var player in _playerDictionary.Values)
        {
            Destroy(player.gameObject);
        }
        _playerDictionary.Clear();
    }
    
    private void OnDestroy()
    {
        SessionClient.Instance.OnUnityPlayerData -= UpdateNetworkPlayers;
        
        ClearPlayers();
    }
    
    private void UpdateNetworkPlayers(UnityPlayersOut incomingPlayersData)
    {
        // Find players that are not in the incoming data
        var playerIds = new HashSet<string>(incomingPlayersData.Players.Select(x => x.Id));
        
        // Also filter out the local player from the dictionary
        playerIds.Remove(SessionClient.Instance.ClientId);
        
        // Compare the current player dictionary with the incoming player data and remove players that are not in the incoming data
        var playersToRemove = _playerDictionary.Values.Where(x => !playerIds.Contains(x.Id)).ToList();
        foreach (var player in playersToRemove)
        {
            RemovePlayer(player);
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
            // Create a new player instance
            NetworkPlayer newPlayer = Instantiate(networkPlayerPrefab).GetComponent<NetworkPlayer>();
            newPlayer.Id = playerData.Id;
            newPlayer.SetNameCard(playerData.Name);
            newPlayer.SetColor(playerData.Color);
            _playerDictionary.Add(newPlayer.Id, newPlayer);
            
            // Create cursor for the player
            NetworkVNCCursor cursor = Instantiate(playerCursorPrefab, newPlayer.transform).GetComponent<NetworkVNCCursor>();
            cursor.Color = newPlayer.color;
            cursor.PlayerNameLabel = playerData.Name;
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
