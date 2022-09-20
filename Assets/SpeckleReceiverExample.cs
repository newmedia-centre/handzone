using System.Collections;
using System.Collections.Generic;
using Speckle.ConnectorUnity;
using Unity.VisualScripting;
using UnityEngine;

public class SpeckleReceiverExample : MonoBehaviour
{
    private Receiver receiver;
    
    // Start is called before the first frame update
    void Start()
    {
        receiver = this.AddComponent<Receiver>();
        // https://speckle.xyz/streams/a042fe5f43 
        receiver.Init("a042fe5f43");
        receiver.Receive();
    }
}
