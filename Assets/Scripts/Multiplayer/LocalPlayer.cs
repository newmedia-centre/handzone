using System.Collections;
using System.Collections.Generic;
using Schema.Socket.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class LocalPlayer : MonoBehaviour
{
    public Transform hmdRef;
    public Transform leftControllerRef;
    public Transform rightControllerRef;
    
    public static LocalPlayer Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple instances of LocalPlayer detected. Destroying the duplicate instance.");
            Destroy(gameObject);
        }
        
        if(hmdRef == null)
        {
            Debug.LogError("HMD reference not set.");
        }
        if(leftControllerRef == null)
        {
            Debug.LogError("Left controller reference not set.");
        }
        if(rightControllerRef == null)
        {
            Debug.LogError("Right controller reference not set.");
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    public void SendPlayerData(UnityPlayerIn playerIn)
    {
        SessionClient.Instance.SendUnityPlayerIn(playerIn);
    }

    // Update is called once per frame
    void Update()
    {
        // Create new player data to send to the server
        UnityPlayerIn playerIn = new UnityPlayerIn
        {
            Hmd = Utility.TransformToSixDofPosition(hmdRef.transform),
            Left = Utility.TransformToSixDofPosition(leftControllerRef.transform),
            Right = Utility.TransformToSixDofPosition(rightControllerRef.transform)
        };
        
        // Send local player data to the server
        SendPlayerData(playerIn);
    }
}
