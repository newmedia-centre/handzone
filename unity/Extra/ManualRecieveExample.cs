using System.Collections;
using System.Threading.Tasks;
using Speckle.ConnectorUnity;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Transports;
using UnityEngine;

[RequireComponent(typeof(RecursiveConverter))]
public class ManualReceive : MonoBehaviour
{
    
    public string authToken;
    public string serverUrl;
    public string streamId, objectId;
    
    private RecursiveConverter receiver;


    void Awake()
    {
        receiver = GetComponent<RecursiveConverter>();
    }


    IEnumerator Start()
    {
        Debug.developerConsoleVisible = true;
        if(Time.timeSinceLevelLoad > 20) yield return null;
        Receive();
    }

    public void Receive()
    {
        var account = new Account()
        {
            token = authToken,
            serverInfo = new ServerInfo() {url = serverUrl},
        };

        Task.Run(async () =>
        {
            var transport = new ServerTransport(account, streamId);
            var localTransport = new MemoryTransport();
        
            var @base = await Operations.Receive(
                objectId,
                remoteTransport: transport,
                localTransport: localTransport,
                onErrorAction: (m, e)=> Debug.LogError(m + e),
                disposeTransports: true
            );

            Dispatcher.Instance().Enqueue(() =>
            {
                var parentObject = new GameObject(name);
                receiver.RecursivelyConvertToNative(@base, parentObject.transform);

                Debug.Log($"Receive {objectId} completed");
            });
        });
    }
}

