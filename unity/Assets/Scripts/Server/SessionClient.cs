/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using Schema.Socket.Grasshopper;
using Schema.Socket.Index;
using Schema.Socket.Interfaces;
using Schema.Socket.Internals;
using Schema.Socket.Realtime;
using Schema.Socket.Unity;
using SocketIO.Serializer.NewtonsoftJson;
using SocketIOClient;
using UnityEngine;

#endregion

/// <summary>
/// The SessionClient class manages the connection and communication with a
/// server for real-time data exchange related to robotic sessions. It handles
/// the initialization of the Socket.IO client, manages the state of the
/// connection, and processes incoming data from the server.
/// </summary>
public class SessionClient : MonoBehaviour
{
    [HideInInspector] public string url;

    private SocketIOClient.SocketIO _client;
    private Queue<RealtimeDataOut> _dataQueue;
    private Texture2D _cameraFeedTexture;
    private bool _digitalOutput;
    private RobotSession _currentRobotSession;
    private UnityPendantOut _pendantData;

    public MemoryStream vncStream { get; private set; }
    public string ClientId => _client?.Id;
    public string PendantOwner => _pendantData?.Owner;
    public bool IsConnected => _client.Connected;

    public event Action<RealtimeDataOut> OnRealtimeData;
    public event Action<string, Texture2D> OnCameraFeed;
    public event Action<bool> OnDigitalOutputChanged;
    public event Action<string> OnUnityMessage;
    public event Action<UnityPlayersOut> OnUnityPlayerData;
    public event Action<UnityPendantOut> OnUnityPendant;
    public event Action<InternalsGetInverseKinCallback> OnKinematicCallback;
    public event Action<string> OnPlayerInvitation;
    public event Action<GrasshopperMeshesIn> OnGHMeshes;
    public event Action OnConnected;
    public event Action OnDisconnected;

    public static SessionClient Instance { get; private set; }

    /// <summary>
    /// Initializes the SessionClient and sets up the connection to the server.
    /// This method is called when the script instance is being loaded.
    /// It ensures that only one instance of the SessionClient exists and
    /// initializes necessary components for communication.
    /// </summary>
    private void Awake()
    {
        if (GlobalClient.Instance == null)
        {
            Debug.LogWarning("GlobalClient instance is null. Make sure to have a GlobalClient instance in the scene. " +
                             "SessionClient will not be created ");
            return;
        }

        url = GlobalClient.Instance.url + GlobalClient.Instance.Session?.Robot.Name;

        // Create a new Socket.IO client with an authentication token from the global client
        _client = new SocketIOClient.SocketIO(url, new SocketIOOptions
        {
            Auth = new
            {
                token = GlobalClient.Instance.Session?.Token,
                type = "vr"
            }
        });

        // Setup the JSON serializer to handle object references
        _client.Serializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore
        });

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private async void Start()
    {
        _cameraFeedTexture = new Texture2D(2, 2);
        vncStream = new MemoryStream();
        _dataQueue = new Queue<RealtimeDataOut>();

        if (GlobalClient.Instance == null)
        {
            Debug.LogWarning("GlobalClient instance is null. Make sure to have a GlobalClient instance in the scene.");
            return;
        }

        if (GlobalClient.Instance.Session == null)
        {
            Debug.LogWarning("No session is currently active. Make sure to have an active session.");
            return;
        }

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
            var data = _dataQueue.Dequeue();
            if (data == null) return;

            OnRealtimeData?.Invoke(data);
        }
    }

    /// <summary>
    /// Attempts to establish a connection to the server session.
    /// This method registers various event handlers for connection, disconnection,
    /// and error events, as well as specific events for video feeds, Grasshopper meshes,
    /// and Unity-related messages. It also initiates the connection process asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation of connecting to the session.</returns>
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
                var cameraName = response.GetValue<string>();
                var base64 = response.GetValue<string>(1);
                if (_cameraFeedTexture.LoadImage(Convert.FromBase64String(base64)))
                    OnCameraFeed?.Invoke(cameraName, _cameraFeedTexture);
            });
        });

        // Register events for the web client that are specific to Grasshopper

        # region Grasshopper events

        _client.On("grasshopper:meshes", response =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log("Received meshes from server");
                var data = response.GetValue<GrasshopperMeshesIn>();
                if (data == null) return;

                OnGHMeshes?.Invoke(data);
            });
        });

        _client.On("realtime:data", response =>
        {
            var data = response.GetValue<RealtimeDataOut>();
            if (data == null) return;

            _dataQueue.Enqueue(data);
        });

        #endregion

        // Register events for the web client that are specific to the Unity client

        #region Unity events

        // Events whenever a session is joined, client receives player data and pendant data
        _client.On("unity:message",
            response =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    OnUnityMessage?.Invoke(response.GetValue<string>());
                });
            });

        _client.On("unity:players", response =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                var players = response.GetValue<UnityPlayersOut>();
                if (players == null) return;

                OnUnityPlayerData?.Invoke(players);
            });
        });

        _client.On("unity:invite",
            response =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    OnPlayerInvitation?.Invoke(response.GetValue<string>());
                });
            });

        _client.On("unity:pendant", response =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                _pendantData = response.GetValue<UnityPendantOut>();
                OnUnityPendant?.Invoke(_pendantData);
            });
        });

        #endregion

        await _client.ConnectAsync();
    }

    /// <summary>
    /// Sends a command to the robot to take control permission.
    /// This method emits a message to the server to request control
    /// over the robot's operations.
    /// </summary>
    public void TakeControlPermission()
    {
        _client.EmitAsync("unity:pendant");
    }

    /// <summary>
    /// Sends an inverse kinematics request to the server.
    /// This method emits a request with the provided data and
    /// executes the specified callback function upon receiving a response.
    /// </summary>
    /// <param name="data">The data for the inverse kinematics request.</param>
    /// <param name="function">The callback function to execute upon receiving a response.</param>
    public void SendInverseKinematicsRequest(InternalsGetInverseKinIn data, Action function)
    {
        _client.EmitAsync("internals:get_inverse_kin", response =>
        {
            var success = response.GetValue<bool>(0);

            if (success)
            {
                var inverseKin = response.GetValue<InternalsGetInverseKinCallback>(1);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    OnKinematicCallback?.Invoke(inverseKin);
                    function?.Invoke();
                });
            }
            else
            {
                Debug.LogWarning($"Get Inverse Kinematic Failed: {response.GetValue<string>(1)}");
            }
        }, data);
    }

    /// <summary>
    /// Requests a session token from the server.
    /// This method emits a request for a token and returns a task that represents the asynchronous operation.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, containing the session token.</returns>
    public Task<string> RequestSessionToken()
    {
        var tcs = new TaskCompletionSource<string>();
        _client.EmitAsync("token", response =>
        {
            var success = response.GetValue<bool>(0);

            if (success)
            {
                var token = response.GetValue<string>(1);
                tcs.SetResult(token);
                Debug.Log($"Token: {token}");
            }
            else
            {
                tcs.SetException(new Exception(response.GetValue<string>(1)));
                Debug.Log("Failed to get token");
            }
        });
        return tcs.Task;
    }

    /// <summary>
    /// Sends a linear speed command to the robot.
    /// This method converts the provided translation and rotation data into a format suitable for the robot's motion.
    /// </summary>
    /// <param name="translateDirection">The direction of translation as a Vector3.</param>
    /// <param name="rotateAxis">The axis of rotation as a Vector3.</param>
    /// <param name="a">The acceleration for the motion.</param>
    /// <param name="t">The time duration for the motion.</param>
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

    /// <summary>
    /// Sends a linear speed command to the robot.
    /// This method emits a command with the specified speed vector, acceleration, and time.
    /// </summary>
    /// <param name="xd">The speed vector as an array of doubles.</param>
    /// <param name="a">The acceleration for the motion.</param>
    /// <param name="t">The time duration for the motion.</param>
    public void Speedl(double[] xd, double a, double t)
    {
        _client.EmitAsync("motion:speedl", xd, a, t);
    }

    /// <summary>
    /// Sends a linear movement command to the robot.
    /// This method emits a command to move the robot to the specified pose with given parameters.
    /// </summary>
    /// <param name="pose">The target pose as an array of doubles.</param>
    /// <param name="a">The acceleration for the motion.</param>
    /// <param name="v">The velocity for the motion.</param>
    /// <param name="t">The time duration for the motion.</param>
    /// <param name="r">The radius for the circular motion.</param>
    public void MoveL(double[] pose, double a, double v, double t, double r)
    {
        _client.EmitAsync("motion:movel", pose, a, v, t, r);
    }

    /// <summary>
    /// Sends a joint movement command to the robot.
    /// This method emits a command to move the robot's joints to the specified positions with given parameters.
    /// </summary>
    /// <param name="q">The joint positions as an array of doubles.</param>
    /// <param name="a">The acceleration for the motion.</param>
    /// <param name="v">The velocity for the motion.</param>
    /// <param name="t">The time duration for the motion.</param>
    /// <param name="r">The radius for the circular motion.</param>
    public void MoveJ(double[] q, double a, double v, double t, double r)
    {
        _client.EmitAsync("motion:movej", q, a, v, t, r);
    }

    public void SetToolDigitalOut(int n, bool b)
    {
        _client.EmitAsync("interfaces:set_tool_digital_out", new InterfacesSetToolDigitalOutIn
        {
            B = b,
            N = n
        });
        _digitalOutput = b;
    }

    public void ToggleToolDigitalOut()
    {
        SetToolDigitalOut(0, !_digitalOutput);
        OnDigitalOutputChanged?.Invoke(!_digitalOutput);
    }

    public void SendUnityMessage(string message)
    {
        var data = new { message = message };
        _client.EmitAsync("unity:message", data);
    }

    public void PlayProgram()
    {
        _client.EmitAsync("unity:run", new UnityRunIn
        {
            Run = true
        });
    }

    public void PauseProgram()
    {
        _client.EmitAsync("unity:run", new UnityRunIn
        {
            Run = false
        });
    }

    public void SendUnityPlayerIn(UnityPlayerIn unityPlayer)
    {
        _client.EmitAsync("unity:player", unityPlayer);
    }

    public void SendUnityPendant(Vector6D message)
    {
        _client.EmitAsync("unity:pendant", message);
    }

    public void EmergencyStop()
    {
        _client.EmitAsync("motion:emergency_stop");
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