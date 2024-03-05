using TMPro;
using UnityEngine;

public class OtherPlayerController : MonoBehaviour
{
    [Header("Actual player")]
    public GameObject player;

    [Header("Glasses")]
    public GameObject glasses;

    [Header("Name Card")]
    public GameObject nameCard;
    public TMP_Text playerNameCard;
    
    public void Initialize(string playerName)
    {
        playerNameCard.SetText(playerName);
    }
    
    void Update()
    {
        Vector3 delta = nameCard.transform.position - player.transform.position;

        nameCard.transform.rotation = Quaternion.LookRotation(delta);
    }

    void ChangePlayerTransform(Transform toGotTo)
    {
        transform.position = toGotTo.position;
        glasses.transform.rotation = toGotTo.rotation;
    }
}
