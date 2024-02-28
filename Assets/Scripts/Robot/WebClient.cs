using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using Robots;
using UnityEngine;
using Schema.Socket.Realtime;
using SocketIO.Serializer.NewtonsoftJson;
using UnityEngine.UI;

public class WebClient : MonoBehaviour
{
    public string url = "http://172.19.14.251:3000/172.19.14.251";
    
    private SocketIOClient.SocketIO _client;
    
    // Add a queue to store the incoming data
    private Queue<RealtimeData> _dataQueue = new Queue<RealtimeData>();
    
    // Add a reference to the RhinoCommon assembly
    private Assembly _rhinoCommonAssembly;

    /// <summary>
    /// Gets the list of found robots on server
    /// </summary>
    public string[] Robots { 
        get; 
        private set; 
    }

    // Unity event Actions
    public static event Action<RealtimeData> OnRealtimeData;
    public static event Action<List<IToolpath>> OnToolpaths;
    public static event Action<Texture2D> OnCameraFeed; 
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action OnSessionJoin;
    public event Action OnSessionJoined;
    public event Action OnSessionLeft;
    
    // Singleton instance
    private static WebClient _instance;
    public static WebClient Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<WebClient>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = typeof(WebClient).ToString();
                    _instance = go.AddComponent<WebClient>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// This method is called when the script starts.
    /// It asynchronously tries to connect to the web server by calling the TryConnectToWebServer() method.
    /// If an error occurs, it logs the error message.
    /// </summary>
    private async void Start()
    {
        _client = new SocketIOClient.SocketIO(url);
        var jsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        });
        _client.Serializer = jsonSerializer;
        
        // For testing purposes only!!! 
        try
        {
            await TryConnectToWebServer();
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
        }
    }
    
    // Add a new method to process the data in the queue
    private void Update()
    {
        if (_dataQueue.Count > 0)
        {
            RealtimeData data = _dataQueue.Dequeue();
            if (data == null) return;
                
            OnRealtimeData?.Invoke(data);
        }
    }

    /// <summary>
    /// This method establishes a connection to the web server and sets up event handlers for receiving real-time data
    /// and handling the connection status.
    /// It emits a motion command to the server after the connection is established.
    /// </summary>
    public async Task TryConnectToWebServer()
    {
        Debug.Log("Connecting to web server...");
        
        // Register actions
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

        _client.OnError += (sender, s) =>
        {
            Debug.Log($@"Received error from server: {s}");
        };
        
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
                    // TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
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
            
            // Instead of invoking the event immediately, enqueue the data
            _dataQueue.Enqueue(data);
        });
        
        _client.On("video", response =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Texture2D texture2D = new Texture2D(2, 2);
                var base64 = response.GetValue<string>();
                texture2D.LoadImage(Convert.FromBase64String(base64));
                OnCameraFeed?.Invoke(texture2D);
            });
        });

        // Connect to server asynchronously 
        await _client.ConnectAsync();
    }

    /// <summary>
    /// Attempt to join a session by id.  
    /// </summary>
    /// <param name="id"></param>
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
    
    /// <summary>
    /// Send a speedl command to the robot.
    /// </summary>
    /// <param name="xd">Tool speed in m/s (spatial vector)</param>
    /// <param name="a">Tool position acceleration</param>
    /// <param name="t">Time (s) before function returns (optional)</param>
    public void Speedl(double[] xd, double a, double t)
    {
        _client.EmitAsync("motion:speedl", xd, a, t);
    }

    public async Task MoveJ(double[] q, double a, double v, double t, double r)
    {
        await _client.EmitAsync("motion:movej", q, a, v, t, r);
    }
    
    public async Task SetDigitalOut(int n, bool b)
    {
        await _client.EmitAsync("motion:setdigitalout", n, b);
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

