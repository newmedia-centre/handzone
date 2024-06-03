using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using Schema.Socket.Index;
using UnityEngine;
using Schema.Socket.Realtime;
using Schema.Socket.Unity;
using SocketIO.Serializer.NewtonsoftJson;
using Schema.Socket.Internals;
using SocketIOClient;

public class SessionClient : MonoBehaviour
{
    [HideInInspector] public string url;

    private SocketIOClient.SocketIO _client;
    private Queue<RealtimeDataOut> _dataQueue;
    private Texture2D _cameraFeedTexture;
    private bool _digitalOutput;
    private RobotSession _currentRobotSession;

    public MemoryStream vncStream { get; private set; }
    public string ClientId => _client?.Id;

    public event Action<RealtimeDataOut> OnRealtimeData;
    public event Action<Texture2D> OnCameraFeed;
    public event Action<bool> OnDigitalOutputChanged;
    public event Action<string> OnUnityMessage;
    public event Action<UnityPlayersOut> OnUnityPlayerData;
    public event Action<UnityPendantOut> OnUnityPendant;
    public event Action<InternalsGetInverseKinCallback> OnKinematicCallback;
    public event Action<string> OnPlayerInvitation;
    public event Action OnConnected;
    public event Action OnDisconnected;

    public static SessionClient Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        _cameraFeedTexture = new Texture2D(2, 2);
        vncStream = new MemoryStream();
        _dataQueue = new Queue<RealtimeDataOut>();
        
        if (GlobalClient.Instance == null)
        {
            Debug.LogError("GlobalClient instance is null. Make sure to have a GlobalClient instance in the scene.");
            return;
        }

        if (GlobalClient.Instance.Session == null)
        {
            Debug.LogError("No session is currently active. Make sure to have an active session.");
            return;
        }
        
        url = GlobalClient.Instance.url + GlobalClient.Instance.Session?.Robot.Name;
        
        // Create a new Socket.IO client with an authentication token from the global client
        _client = new SocketIOClient.SocketIO(url, new SocketIOOptions
        {
            Auth = new { token = GlobalClient.Instance.Session?.Token }
        });
        
        // Setup the JSON serializer to handle object references
        _client.Serializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        });

        // Attempt to connect to the session
        try
        {
            await TryConnectToSession();
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
        }
    }

    private void Update()
    {
        if (_dataQueue.Count > 0)
        {
            RealtimeDataOut data = _dataQueue.Dequeue();
            if (data == null) return;

            OnRealtimeData?.Invoke(data);
        }
    }

    public async Task TryConnectToSession()
    {

        // Register general events for the web client, such as connection, disconnection, and errors

        #region General connection events

        Debug.Log("Connecting to session...");

        _client.OnConnected += (sender, args) =>
        {
            Debug.Log("Connected to session");
            OnConnected?.Invoke();
        };

        _client.OnDisconnected += (sender, s) =>
        {
            Debug.Log("Disconnected from session");
            OnDisconnected?.Invoke();
        };

        _client.OnError += (sender, s) => { Debug.Log($@"Received error from server: {s}"); };

        #endregion

        // Register events for the web client that are specific to the video feed
        _client.On("video", response =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                // Index 0 = Camera name | Index 1 = Base64 encoded image
                var base64 = response.GetValue<string>(1);
                _cameraFeedTexture.LoadImage(Convert.FromBase64String(base64));
                OnCameraFeed?.Invoke(_cameraFeedTexture);
            });
        });

        // Register events for the web client that are specific to Grasshopper

        # region Grasshopper events

        _client.On("grasshopper:program", response =>
        {
            Debug.Log("Received program from server...");
        });

        _client.On("realtime:data", response =>
        {
            RealtimeDataOut data = response.GetValue<RealtimeDataOut>();
            if (data == null) return;

            _dataQueue.Enqueue(data);
        });

        #endregion

        // Register events for the web client that are specific to the Unity client

        #region Unity events

        // Events whenever a session is joined, client receives player data and pendant data
        _client.On("unity:message", response =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log("Hello from Web server! " +  response.GetValue<string>());
                OnUnityMessage?.Invoke(response.GetValue<string>());
            });
        });
        
        _client.On("unity:players", response =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                OnUnityPlayerData?.Invoke(response.GetValue<UnityPlayersOut>());
            });
        });

        _client.On("unity:invite", response =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log("Received invitation from server...");
                OnPlayerInvitation?.Invoke(response.GetValue<string>());
            });
        });
        
        _client.On("unity:pendant", response =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log("received pendant data from server...");
                OnUnityPendant?.Invoke(response.GetValue<UnityPendantOut>());
            });
        });
        #endregion

        await _client.ConnectAsync();
    }

    public void SendInverseKinematicsRequest(InternalsGetInverseKinIn data, Action function)
    {
        _client.EmitAsync("internals:get_inverse_kin", response =>
        {
            OnKinematicCallback?.Invoke(response.GetValue<InternalsGetInverseKinCallback>());
            function?.Invoke();
        }, data);
    }
    
    public void Speedl(Vector3 translateDirection, Vector3 rotateAxis, float a, float t)
    {
        double[] xd =
        {
            -translateDirection.z,
            translateDirection.x,
            translateDirection.y,
            -rotateAxis.z,
            rotateAxis.x,
            rotateAxis.y
        };
        Speedl(xd, a, t);
    }

    public void SetTCP(double[] pose)
    {
        _client.EmitAsync("motion:set_tcp", pose);
    }

    public void Speedl(double[] xd, double a, double t)
    {
        _client.EmitAsync("motion:speedl", xd, a, t);
    }

    public void MoveL(double[] pose, double a, double v, double t, double r)
    {
        _client.EmitAsync("motion:movel", pose, a, v, t, r);
    }

    public void MoveJ(double[] q, double a, double v, double t, double r)
    {
        _client.EmitAsync("motion:movej", q, a, v, t, r);
    }

    public void SetToolDigitalOut(int n, bool b)
    {
        _client.EmitAsync("interfaces:set_tool_digital_out", n, b);
    }
    
    public bool ToggleToolDigitalOut(bool value)
    {
        SetToolDigitalOut(0, !value);
        OnDigitalOutputChanged?.Invoke(!value);
        return !value;
    }
    
    public void SendUnityMessage(string message)
    {
        var data = new { message = message };
        _client.EmitAsync("unity:message", data);
    }
    
    public void SendUnityPlayerIn(UnityPlayerIn unityPlayer)
    {
        _client.EmitAsync("unity:players", unityPlayer);
    }
    
    public void SendUnityPendant(Vector6D message)
    {
        _client.EmitAsync("unity:pendant", message);
    }

    private async void OnDestroy()
    {
        await vncStream.DisposeAsync();
        
        if (_client != null)
        {
            await _client.DisconnectAsync();
            _client.Dispose();
        }
    }
}

