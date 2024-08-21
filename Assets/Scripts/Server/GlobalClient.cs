using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
    /// Signature.
    /// </summary>
    private static string _signature;

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

    // generate a secure signature to identify the auth flow with
    private static string NewSignature()
    {
        byte[] signature = new byte[32];
            
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(signature);
        }
            
        _signature = Convert.ToBase64String(signature);
        return _signature;
    }

    internal async Task <string> GetPin()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                // encode the data as JSON
                string jsonData = JsonConvert.SerializeObject(new
                {
                    signature = NewSignature()
                });
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    
                // make the POST request and block until the result is returned
                HttpResponseMessage response = await client.PostAsync(url + "api/auth/pin", content);

                // get the pin from the response
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
        catch (HttpRequestException e)
        {
            Debug.LogError($"Request error: {e.Message}");
            throw;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Attempts to connect to the web server using the provided PIN.
    /// Registers connection, disconnection, and error handling events.
    /// </summary>
    /// <param name="pinInput">The PIN used for secure connection.</param>
    public async Task TryConnectToGlobalServer(String pin)
    {
        _client = new SocketIOClient.SocketIO(url, new SocketIOOptions
        {
            Auth = new
            {
                pin,
                signature = _signature
            }
        });
        
        _client.Serializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        });
        
        Debug.Log("Connecting to global server...");
        OnConnecting?.Invoke();

        _client.OnConnected += (sender, args) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log("Connected to global server");
                OnConnected?.Invoke();
            });
        };

        _client.OnDisconnected += (sender, s) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log("Disconnected from global server");
                OnDisconnected?.Invoke();
            });
        };

        _client.OnError += (sender, s) =>
        {
            // check if more time is needed
            if (s == "Pin not claimed")
            {
                Console.WriteLine("Pin not claimed, waiting and retrying...");
                Thread.Sleep(1000);
                _client.ConnectAsync();
            }
            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Debug.Log($@"Received error from global server: {s}");
                    OnError?.Invoke(s);
                });
            }
        };

        _client.On("sessions", response =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                var sessions = response.GetValue<SessionsOut>();
                if (sessions == null)
                {
                    Debug.LogError("Could not parse sessions.");
                    return;
                }
                
                Sessions = sessions;
                OnSessionsReceived?.Invoke(sessions);
                Debug.Log("Sessions received: " + sessions.Sessions.Count);
            });
        });
        
        await _client.ConnectAsync();
    }

    /// <summary>
    /// Requests a virtual robot session from the server and join it.
    /// </summary>
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
                    Debug.Log("Virtual session created and joined." + Session.Robot.Name);
                });           
            }
            else
            {
                Debug.LogWarning("Could not join session.");
            }
        }, "sandbox");
    }
    
    /// <summary>
    /// Join a session with the provided session address.
    /// </summary>
    /// <param name="sessionAddress"></param>
    public void JoinSession(string sessionAddress)
    {
        OnSessionJoin?.Invoke(sessionAddress);
        _client.EmitAsync("join", response =>
        {
            var success = response.GetValue<bool>(0);
            if (success)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Session = response.GetValue<JoinSessionOut>(1);
                    StartCoroutine(LoadSceneCoroutine("Scenes/UR Robot Scene"));
                    OnSessionJoined?.Invoke(Session);
                });
            }
            else
            {
                Debug.LogWarning("Could not join session.");
            }
        }, sessionAddress);
    }
    
    /// <summary>
    /// Loads a scene asynchronously.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
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