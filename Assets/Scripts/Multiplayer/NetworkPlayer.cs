using System.Collections.Generic;
using Schema.Socket.Unity;
using TMPro;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    public string Id { get; set; }
    
    private PlayerData _receivedPlayerData;

    [Header("Client reference")] 
    public GameObject clientHMD;

    [Header("Network Transforms")] 
    public GameObject hmd;
    public GameObject left;
    public GameObject right;
    public NetworkVNCCursor cursor;

    [Header("Name Card")] 
    public GameObject nameCard;
    public TMP_Text playerNameCard;

    [Header("Player material")] 
    public Color color;
    public List<Renderer> playerRenderers;
    
    [Header("Interpolation Settings")]
    public float speed = 5.0F; // Movement speed in units per second.
    public float rotationSpeed = 8.0F; // Rotation speed in degrees per second.

    private void Start()
    {
        // Receive the client HMD by looking for the main camera
        clientHMD = Camera.main.gameObject;
    }

    public void SetNameCard(string playerName)
    {
        playerNameCard.SetText(playerName);
    }

    public void SetColor(string colorString)
    { 
        color = ColorUtility.TryParseHtmlString(colorString, out var newColor) ? newColor : Color.white;

        foreach (var playerRenderer in playerRenderers)
        {
            playerRenderer.material.color = color;
        }
    }

    void Update()
    {
        // This needs to be updated to make it rotate towards a client side
        Vector3 delta = nameCard.transform.position - clientHMD.transform.position;

        nameCard.transform.rotation = Quaternion.LookRotation(delta);
        
        if (_receivedPlayerData == null)
            return;
        
                
        // Define the speed of the interpolation
        float lerpSpeed = speed * Time.deltaTime;
        float rotationLerpSpeed = rotationSpeed * Time.deltaTime;
        
        // Define the target positions and rotations
        Vector3 hmdTargetPosition = new Vector3((float)_receivedPlayerData.Hmd.Position.X, (float)_receivedPlayerData.Hmd.Position.Y, (float)_receivedPlayerData.Hmd.Position.Z);
        Quaternion hmdTargetRotation = Quaternion.Euler(new Vector3((float)_receivedPlayerData.Hmd.Rotation.X, (float)_receivedPlayerData.Hmd.Rotation.Y, (float)_receivedPlayerData.Hmd.Rotation.Z));

        Vector3 leftTargetPosition = new Vector3((float)_receivedPlayerData.Left.Position.X, (float)_receivedPlayerData.Left.Position.Y, (float)_receivedPlayerData.Left.Position.Z);
        Quaternion leftTargetRotation = Quaternion.Euler(new Vector3((float)_receivedPlayerData.Left.Rotation.X, (float)_receivedPlayerData.Left.Rotation.Y, (float)_receivedPlayerData.Left.Rotation.Z));

        Vector3 rightTargetPosition = new Vector3((float)_receivedPlayerData.Right.Position.X, (float)_receivedPlayerData.Right.Position.Y, (float)_receivedPlayerData.Right.Position.Z);
        Quaternion rightTargetRotation = Quaternion.Euler(new Vector3((float)_receivedPlayerData.Right.Rotation.X, (float)_receivedPlayerData.Right.Rotation.Y, (float)_receivedPlayerData.Right.Rotation.Z));
        
        // Interpolate the position and rotation of the hmd, left, and right objects
        hmd.transform.position = Vector3.Lerp(hmd.transform.position, hmdTargetPosition, lerpSpeed);
        hmd.transform.rotation = Quaternion.Lerp(hmd.transform.rotation, hmdTargetRotation, rotationLerpSpeed);

        left.transform.position = Vector3.Lerp(left.transform.position, leftTargetPosition, lerpSpeed);
        left.transform.rotation = Quaternion.Lerp(left.transform.rotation, leftTargetRotation, rotationLerpSpeed);

        right.transform.position = Vector3.Lerp(right.transform.position, rightTargetPosition, lerpSpeed);
        right.transform.rotation = Quaternion.Lerp(right.transform.rotation, rightTargetRotation, rotationLerpSpeed);

        if (_receivedPlayerData.Cursor.X > 0.001f)
        {
            Vector2 cursorPosition = new Vector2((float)_receivedPlayerData.Cursor.X, (float)_receivedPlayerData.Cursor.Y);
            cursor.transform.localPosition = Vector3.Lerp(cursor.transform.localPosition, cursorPosition, lerpSpeed);
        }
    }

    public void UpdatePlayerData(PlayerData playerData)
    {
        _receivedPlayerData = playerData;
    }
}