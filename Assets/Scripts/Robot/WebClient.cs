using System;
using System.Threading.Tasks;
using UnityEngine;
using SocketIO.Serializer.NewtonsoftJson;
using Schema.Socket.Realtime;

public class WebClient : MonoBehaviour
{
    [SerializeField] private string url = "http://172.19.14.253:3000/172.19.14.251";
    private SocketIOClient.SocketIO _client;

    /// <summary>
    /// Gets the list of found robots on server
    /// </summary>
    public string[] Robots { 
        get; 
        private set; 
    }

    // Unity event Actions
    public event Action<RealtimeData> OnRealtimeData;
    public event Action<string[]> OnRobots;
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action OnSessionJoin;
    public event Action OnSessionJoined;
    public event Action OnSessionLeft;

    private WebClient()
    {
        _client = new SocketIOClient.SocketIO(url);
        var jsonSerializer = new NewtonsoftJsonSerializer();
        _client.Serializer = jsonSerializer;
    }

    /// <summary>
    /// This method is called when the script starts.
    /// It asynchronously tries to connect to the web server by calling the TryConnectToWebServer() method.
    /// If an error occurs, it logs the error message.
    /// </summary>
    private async void Start()
    {
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
        
        _client.On("realtime:data", response =>
        {
            RealtimeData data = response.GetValue<RealtimeData>();
            if (data != null)
                OnRealtimeData?.Invoke(data);
        });
        
        // TODO: Update event name to the configured server side invoker for robots connected
        _client.On("robots", response =>
        {
            Robots = response.GetValue<string[]>();
            OnRobots?.Invoke(Robots);
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
    
    public async Task Speedl(Vector3 translateDirection, Vector3 rotateAxis, float a, float t)
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
        await Speedl(xd, a, t);
    }
    
    /// <summary>
    /// Send a speedl command to the robot.
    /// </summary>
    /// <param name="xd">Tool speed in m/s (spatial vector)</param>
    /// <param name="a">Tool position acceleration</param>
    /// <param name="t">Time (s) before function returns (optional)</param>
    public async Task Speedl(double[] xd, double a, double t)
    {
        await _client.EmitAsync("motion:speedl", xd, a, t);
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

