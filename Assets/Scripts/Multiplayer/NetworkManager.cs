using System.Collections.Generic;
using Schema.Socket.Unity;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public GameObject playerPrefab;
    
    private List<NetworkPlayer> players = new();
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        WebClient.OnUnityPendant += OnUnityPendant;
        WebClient.OnUnityPlayerData += UnityPlayerData;
    }

    private void OnUnityPendant(UnityPendantIn obj)
    {
        Debug.Log("Pendant data received");
    }

    private void UnityPlayerData(PlayerData obj)
    {
        // Check the player list for the id
        if (players.Find(x => x.Id == obj.Id) == null)
        {
            SpawnPlayer(obj);
        }
        else
        {
            NetworkPlayer player = players.Find(x => x.Id == obj.Id);

            // Despawn the player if the player is not found
            if (player == null)
            {
                DespawnPlayer();
                return;
            };

            // Update the player position
            if (player != null)
                player.UpdatePositions(obj);
        }
        
    }

    public void SpawnPlayer(PlayerData obj)
    {
        NetworkPlayer newPlayer = Instantiate(playerPrefab).GetComponent<NetworkPlayer>();
        newPlayer.Id = obj.Id;
        newPlayer.SetNameCard(obj.Name);
        newPlayer.SetColor(obj.Color);
        players.Add(newPlayer);
    }
    
    public void DespawnPlayer()
    {
        Destroy(playerPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
