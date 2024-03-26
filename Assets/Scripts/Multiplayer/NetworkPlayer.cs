using System;
using System.Collections;
using Schema.Socket.Unity;
using TMPro;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    public string Id { get; set; }

    [Header("Client reference")] public GameObject clientHMD;

    [Header("Network Transforms")] public GameObject hmd;
    public GameObject left;
    public GameObject right;

    [Header("Name Card")] public GameObject nameCard;
    public TMP_Text playerNameCard;

    [Header("Player material")] public string color;
    public Renderer playerRenderer;

    private void Start()
    {
        // Receive the client HMD by looking for the main camera
        clientHMD = Camera.main.gameObject;

        playerRenderer = hmd.GetComponent<Renderer>();
    }

    public void SetNameCard(string playerName)
    {
        playerNameCard.SetText(playerName);
    }

    public void SetColor(string color)
    {
        this.color = color;
        playerRenderer.material.color =
            ColorUtility.TryParseHtmlString(color, out var newColor) ? newColor : Color.white;
    }

    void Update()
    {
        // This needs to be updated to make it rotate towards a client side
        Vector3 delta = nameCard.transform.position - clientHMD.transform.position;

        nameCard.transform.rotation = Quaternion.LookRotation(delta);
    }

    IEnumerator ExampleCoroutine()
    {
        WebClient.Instance.SendUnityMessage("Hello from Unity client");
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1f);
    }

    public void UpdatePositions(PlayerData playerData)
    {
        hmd.transform.position = new((float)playerData.Hmd.Position.X, (float)playerData.Hmd.Position.Y, (float)playerData.Hmd.Position.Z);
        hmd.transform.eulerAngles = new((float)playerData.Hmd.Rotation.X, (float)playerData.Hmd.Rotation.Y, (float)playerData.Hmd.Rotation.Z);

        left.transform.position = new((float)playerData.Left.Position.X, (float)playerData.Left.Position.Y, (float)playerData.Left.Position.Z);
        left.transform.eulerAngles = new((float)playerData.Left.Rotation.X, (float)playerData.Left.Rotation.Y, (float)playerData.Left.Rotation.Z);
        
        right.transform.position = new((float)playerData.Right.Position.X, (float)playerData.Right.Position.Y, (float)playerData.Right.Position.Z);
        right.transform.eulerAngles = new((float)playerData.Right.Rotation.X, (float)playerData.Right.Rotation.Y, (float)playerData.Right.Rotation.Z);
    }
}