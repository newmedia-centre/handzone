using Schema.Socket.Unity;
using UnityEngine;

/// <summary>
/// This class is used to modify the sprite rendered color on the permission outline frame.
/// It is used to indicate the permission status of controlling the robot of the user.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PermissionFrame : MonoBehaviour
{
    public Color accessGrantedColor = Color.green;
    public Color accessDeniedColor = Color.red;
    
    private SpriteRenderer _spriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        if(SessionClient.Instance == null)
        {
            Debug.LogWarning("SessionClient instance is null. Make sure to have a SessionClient instance in the scene.");
            return;
        }
        
        SessionClient.Instance.OnUnityPendant += UpdatePermissionFrame;
    }

    private void UpdatePermissionFrame(UnityPendantOut pendantOut)
    {
        // Check if the current user is the owner of the pendant and change the color of the frame accordingly
        if(SessionClient.Instance.ClientId != pendantOut.Owner)
        {
            _spriteRenderer.color = accessDeniedColor;
        }
        else if (SessionClient.Instance.ClientId == pendantOut.Owner)
        {
            _spriteRenderer.color = accessGrantedColor;
        }
    }

    private void OnDestroy()
    {
        if(SessionClient.Instance)
            SessionClient.Instance.OnUnityPendant -= UpdatePermissionFrame;
    }
}
