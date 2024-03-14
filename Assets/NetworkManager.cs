using System.Collections.Generic;
using UnityEngine;
using XRZone;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public GameObject playerPrefab;
    
    private List<NetworkPlayer> players = new List<NetworkPlayer>();
    
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
        WebClient.OnUnityPosition += OnUnityPosition;
    }

    private void OnUnityPendant(PendantDataOut obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnUnityPosition(PositionDataOut obj)
    {
        throw new System.NotImplementedException();
    }

    public void SpawnPlayer()
    {
        Instantiate(playerPrefab);
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
