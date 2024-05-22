using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using Schema.Socket.Index;
using UnityEngine;
using SocketIO.Serializer.NewtonsoftJson;
using SocketIOClient;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the global client connections and interactions with a web server using Socket.IO.
/// This class handles connection lifecycle events, session management, and robot data reception.
/// </summary>
public class GlobalClient : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of GlobalClient.
    /// </summary>
    public static GlobalClient Instance { get; private set; }
    
    /// <summary>
    /// URL of the web server to connect to.
    /// </summary>
    public string url;
    
    public JoinSessionOut Session { get; private set; }
    
    /// <summary>
    /// Latest robot data received from the server.
    /// </summary>
    public RobotsOut Robots { get; private set; }

    private SocketIOClient.SocketIO _client;

    // Connection lifecycle events
    public event Action OnConnecting;
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<string> OnError;
    public event Action OnSessionJoin;
    public event Action OnSessionJoined;
    public event Action OnSessionLeft;
    
    // Data reception events
    public event Action<RobotsOut> OnRobotsReceived;
    public event Action<JoinSessionOut> OnSessionReceived;
    
    /// <summary>
    /// Ensures that only one instance of GlobalClient exists within the application.
    /// </summary>
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

    /// <summary>
    /// Attempts to connect to the web server using the provided PIN.
    /// Registers connection, disconnection, and error handling events.
    /// </summary>
    /// <param name="pin">The PIN used for secure connection.</param>
    public async Task TryConnectToWebServer(String pin)
    {
        _client = new SocketIOClient.SocketIO(url, new SocketIOOptions
        {
            Auth = new{pin}
        });
        
        var jsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        });
        
        _client.Serializer = jsonSerializer;
        Debug.Log("Connecting to global server...");
        OnConnecting?.Invoke();

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
            OnError?.Invoke(s);
        };

        _client.On("robots", response =>
        {
            var robots = response.GetValue<RobotsOut>();
            if (robots == null) return;
            
            OnRobotsReceived?.Invoke(robots);
            Debug.Log($"Received robots: {robots.Real?.Address}");
            Debug.Log("Received robots sessions: " + robots.Sessions.Length);
        });

        _client.On("session", response =>
        {
            // Handle session data reception here
        });
        
        await _client.ConnectAsync();
    }
    
    /// <summary>
    /// Joins a session with the specified ID.
    /// </summary>
    /// <param name="id">The session ID to join.</param>
    public async Task JoinSession(string id)
    {
        OnSessionJoin?.Invoke();
        await _client.EmitAsync("join", new { room = id });
        OnSessionJoined?.Invoke();
    }

    public void RequestVirtual()
    {
        _client.EmitAsync("virtual", response =>
        {
            var success = response.GetValue<bool>(0);
            Debug.Log(success);
            if (success)
            {
                Session = response.GetValue<JoinSessionOut>(1);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    SceneManager.LoadScene("Scenes/UR Robot Scene");   
                    OnSessionReceived?.Invoke(Session);
                });           
            }
            else
            {
                Debug.LogError("Could not join session.");
            }
        });
    }
    
    /// <summary>
    /// Leaves a session with the specified ID.
    /// </summary>
    /// <param name="id">The session ID to leave.</param>
    public async Task LeaveSession(string id)
    {
        await _client.EmitAsync("leave", new { room = id });
        OnSessionLeft?.Invoke();
    }

    /// <summary>
    /// Ensures proper disconnection and cleanup of the Socket.IO client upon destruction of the object.
    /// </summary>
    private async void OnDestroy()
    {
        if (_client != null)
        {
            await _client.DisconnectAsync();
            _client.Dispose();
        }
    }
}