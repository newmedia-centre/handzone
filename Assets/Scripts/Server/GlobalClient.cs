using System;
using System.Collections;
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
    public SessionsOut Sessions { get; private set; }

    private SocketIOClient.SocketIO _client;

    // Connection lifecycle events
    public event Action OnConnecting;
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<string> OnError;
    
    // Data reception events
    public event Action<SessionsOut> OnSessionsReceived;
    public event Action<string> OnSessionJoin;
    public event Action<JoinSessionOut> OnSessionJoined;
    
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
    public async Task TryConnectToGlobalServer(String pin)
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

        _client.On("sessions", response =>
        {
            var session = response.GetValue<SessionsOut>();
            if (session == null) return;
            
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                OnSessionsReceived?.Invoke(session);
            });
        });
        
        await _client.ConnectAsync();
    }

    public void RequestVirtual()
    {
        _client.EmitAsync("virtual", response =>
        {
            var success = response.GetValue<bool>(0);
            if (success)
            {
                Session = response.GetValue<JoinSessionOut>(1);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    StartCoroutine(LoadSceneCoroutine("Scenes/UR Robot Scene"));
                    OnSessionJoined?.Invoke(Session);
                });           
            }
            else
            {
                Debug.LogWarning("Could not join session.");
            }
        });
    }
    
    public void JoinSession(string sessionAddress)
    {
        OnSessionJoin?.Invoke(sessionAddress);
        _client.EmitAsync("join", response =>
        {
            var success = response.GetValue<bool>(0);
            if (success)
            {
                Session = response.GetValue<JoinSessionOut>(1);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    StartCoroutine(LoadSceneCoroutine("Scenes/UR Robot Scene"));
                    OnSessionJoined?.Invoke(Session);
                });
            }
            else
            {
                Debug.LogWarning("Could not join session.");
            }
        });
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Start loading the scene
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
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