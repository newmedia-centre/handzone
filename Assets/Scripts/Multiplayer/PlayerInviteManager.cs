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
        
        if(SessionClient.Instance == null)
        {
            Debug.LogWarning("SessionClient instance is null. Make sure to have a SessionClient instance in the scene.");
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
