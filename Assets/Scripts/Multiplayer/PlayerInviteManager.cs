using System.Collections.Generic;
using UnityEngine;

public class PlayerInviteManager : MonoBehaviour
{
    public GameObject playerInvitePrefab;
    public Transform playerInvitesAnchor;

    private List<GameObject> playerInvites;
    
    // Start is called before the first frame update
    void Start()
    {
        if(playerInvitePrefab == null)
            Debug.LogError("Player invite prefab not set..");
        
        if(playerInvitesAnchor == null)
            Debug.LogWarning("Player invites anchor is not set, invites will not be parented.");
        
        SessionClient.Instance.OnPlayerInvitation += InstanceOnOnPlayerInvitation;
    }

    private void InstanceOnOnPlayerInvitation(string playerId)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
