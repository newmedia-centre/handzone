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

    public void UpdatePositions(PlayerDataHmd hmd, PlayerDataLeft left, PlayerDataRight right)
    {
        this.hmd.transform.position = new((float)hmd.Position.X, (float)hmd.Position.Y, (float)hmd.Position.Z);
        this.hmd.transform.eulerAngles = new((float)hmd.Rotation.X, (float)hmd.Rotation.Y, (float)hmd.Rotation.Z);

        this.left.transform.position = new((float)left.Position.X, (float)left.Position.Y, (float)left.Position.Z);
        this.left.transform.eulerAngles = new((float)left.Rotation.X, (float)left.Rotation.Y, (float)left.Rotation.Z);
        
        this.right.transform.position = new((float)right.Position.X, (float)right.Position.Y, (float)right.Position.Z);
        this.right.transform.eulerAngles = new((float)right.Rotation.X, (float)right.Rotation.Y, (float)right.Rotation.Z);
    }
}