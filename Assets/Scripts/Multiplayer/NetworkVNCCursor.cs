using Schema.Socket.Unity;
using UnityEngine;

public class NetworkVNCCursor : MonoBehaviour
{
    void Start()
    {
        SessionClient.Instance.OnUnityPendant += UpdateVNCCursor;
    }

    private void UpdateVNCCursor(UnityPendantOut pendantOut)
    {
        // Check if the pendant data is from this client
        if(pendantOut.Owner == SessionClient.Instance.ClientId)
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnDestroy()
    {
        SessionClient.Instance.OnUnityPendant -= UpdateVNCCursor;
    }
}
