using Schema.Socket.Unity;
using UnityEngine;
using VNCScreen;

public class LocalPlayer : MonoBehaviour
{
    public Transform hmdRef;
    public Transform leftControllerRef;
    public Transform rightControllerRef;
    public VNCXRRaycaster cursorRef;
    
    private float _sendInterval = 0.1f;
    private float _sendTimer = 0.0f;
    
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
        if (SessionClient.Instance != null && SessionClient.Instance.IsConnected)
        {
            // Send the player data to the server with a time interval
            if (_sendTimer < _sendInterval)
            {
                _sendTimer += Time.deltaTime;
                return;
            }
            
            SessionClient.Instance.SendUnityPlayerIn(playerIn);
            _sendTimer = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Create new player data to send to the server
        UnityPlayerIn playerIn = new UnityPlayerIn
        {
            Hmd = Utility.TransformToSixDofPosition(hmdRef.transform),
            Left = Utility.TransformToSixDofPosition(leftControllerRef.transform),
            Right = Utility.TransformToSixDofPosition(rightControllerRef.transform),
            Cursor = cursorRef.TextureCoord
        };

        // Send local player data to the server
        SendPlayerData(playerIn);
    }
}
