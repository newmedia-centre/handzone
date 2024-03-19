using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using Robots;
using Schema.Socket.Motion;
using UnityEngine;
using Schema.Socket.Realtime;
using Schema.Socket.Unity;
using SocketIO.Serializer.NewtonsoftJson;

public class WebClient : MonoBehaviour
{
    [HideInInspector] public string url;

    private SocketIOClient.SocketIO _client;
    private Queue<RealtimeData> _dataQueue;
    private Texture2D _cameraFeedTexture;
    private bool _digitalOutput;

    public string[] Robots { get; private set; }

    public static event Action<RealtimeData> OnRealtimeData;
    public static event Action<List<IToolpath>> OnToolpaths;
    public static event Action<Texture2D> OnCameraFeed;
    public static event Action<bool> OnDigitalOutputChanged;
    public static event Action<string> OnUnityMessage;
    public static event Action<PlayerData> OnUnityPlayerData;
    public static event Action<UnityPendantIn> OnUnityPendant;

    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action OnSessionJoin;
    public event Action OnSessionJoined;
    public event Action OnSessionLeft;

    public static WebClient Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        _cameraFeedTexture = new Texture2D(2, 2);

        _dataQueue = new Queue<RealtimeData>();
        _client = new SocketIOClient.SocketIO(url);
        var jsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        });
        _client.Serializer = jsonSerializer;

        try
        {
            await TryConnectToWebServer();
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
            RealtimeData data = _dataQueue.Dequeue();
            if (data == null) return;

            OnRealtimeData?.Invoke(data);
        }
    }

    public async Task TryConnectToWebServer()
    {

        // Register general events for the web client, such as connection, disconnection, and errors

        #region General connection events

        Debug.Log("Connecting to web server...");

        _client.OnConnected += (sender, args) =>
        {
            Debug.Log("Connected to server");
            OnConnected?.Invoke();
        };

        _client.OnDisconnected += (sender, s) =>
        {
            Debug.Log("Disconnected from server");
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
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                var json = response.GetValue<string>();
                var serializer = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                    FloatParseHandling = FloatParseHandling.Double,
                    TypeNameHandling = TypeNameHandling.Objects,
                };

                Debug.Log("Received program from server...");

                var deserializedProgram = JsonConvert.DeserializeObject<List<IToolpath>>(json, serializer);
                Debug.Log("Received toolpaths from server...");
                OnToolpaths?.Invoke(deserializedProgram);
            });
        });

        _client.On("realtime:data", response =>
        {
            RealtimeData data = response.GetValue<RealtimeData>();
            if (data == null) return;

            _dataQueue.Enqueue(data);
        });

        #endregion

        // Register events for the web client that are specific to the Unity client

        #region Unity events

        _client.On("unity:message", response =>
        {

            Debug.Log("Hello from Web server! " +  response.GetValue<string>());
            OnUnityMessage?.Invoke(response.GetValue<string>());
        });
        
        _client.On("unity:player", response =>
        {
            OnUnityPlayerData?.Invoke(response.GetValue<PlayerData>());
        });
        
        _client.On("unity:pendant", response =>
        {
            OnUnityPendant?.Invoke(response.GetValue<UnityPendantIn>());
        });
        #endregion

        await _client.ConnectAsync();
    }

    public async Task JoinSession(string id)
    {
        OnSessionJoin?.Invoke();
        await _client.EmitAsync("join", new { room = id });
        OnSessionJoined?.Invoke();
    }

    public async Task LeaveSession(string id)
    {
        await _client.EmitAsync("leave", new { room = id });
        OnSessionLeft?.Invoke();
    }

    public void SendInverseKinematicsRequest()
    {
        double[] x = { 0.1,.2,.2,0,3.14,0 };
        var data = new 
        {
            x
        };
        _client.EmitAsync("interfaces:get_inverse_kin", response =>
        { 
            Debug.Log(response);
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
    
    public void SendUnityPosition(UnityPlayerIn unityPlayer)
    {
        _client.EmitAsync("unity:player", unityPlayer);
    }
    
    public void SendUnityPendant(Vector6D message)
    {
        _client.EmitAsync("unity:pendant", message);
    }

    private async void OnDestroy()
    {
        if (_client != null)
        {
            await _client.DisconnectAsync();
            _client.Dispose();
        }
    }
}

