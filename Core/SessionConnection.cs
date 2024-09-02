using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Schema.Socket.Grasshopper;
using Schema.Socket.Index;
using SocketIO.Serializer.NewtonsoftJson;
using SocketIOClient;

namespace Handzone.Core;

public class SessionConnection
{
    /// <summary>
    /// Socket.io client
    /// </summary>
    private SocketIOClient.SocketIO _client;
    
    public bool Connected;
    public JoinSessionOut Session { get; private set; }
    
    // Connection lifecycle events
    public event Action<string> OnError;
    public event Action<string> OnStatus;
    public event Action<bool> OnConnectionChange;
    
    /// <summary>
    /// Attempts to connect to the robot using the provided session info.
    /// Registers connection, disconnection, and error handling events.
    /// </summary>
    /// <param name="session">The session info used for secure connection.</param>
    public async Task TryConnectToSession(JoinSessionOut session)
    {
        Connected = false;
        Session = session;
        OnStatus?.Invoke($"Connecting to robot session: {Session.Robot.Name}...");
        
        Console.WriteLine($"Connecting with token: {session.Token}");
        
        _client = new SocketIOClient.SocketIO(State.Url + session.Robot.Name, new SocketIOOptions
        {
            Auth = new
            {
                token = session.Token
            }
        });
        
        Console.WriteLine("Pass Init");
        
        _client.Serializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        });
        
        Console.WriteLine("Pass Serializer");

        _client.OnConnected += (_, _) =>
        {
            Connected = true;
            OnStatus?.Invoke($"Connected to robot session: {Session.Robot.Name}");
            OnConnectionChange?.Invoke(true);
        };

        _client.OnDisconnected += (_, _) =>
        {
            Connected = false;
            OnStatus?.Invoke($"Disconnected from robot session: {Session.Robot.Name}");
            OnConnectionChange?.Invoke(false);
        };

        _client.OnError += (_, s) =>
        {
            Console.WriteLine(s);
            
            OnError?.Invoke(s);
            OnConnectionChange?.Invoke(false);
        };
        
        Console.WriteLine("Pre Connect");

        await _client.ConnectAsync();
        
        Console.WriteLine("Post Connect");
    }
    
    /// <summary>
    /// Sends an instruction to run the program on the robot.
    /// </summary>
    public async Task Run(GrasshopperRunIn grasshopperRun)
    {
        await _client.EmitAsync("grasshopper:run", grasshopperRun);
    }
    
    /// <summary>
    /// Sends a json payload of the program to Unity to be deserialized as a program.
    /// </summary>
    public async Task Program(GrasshopperProgramIn grasshopperProgramIn)
    {
        await _client.EmitAsync("grasshopper:program", grasshopperProgramIn);
    }
    
    /// <summary>
    /// Sends meshes to the robot session.
    /// </summary>
    public async Task Meshes(GrasshopperMeshesIn grasshopperMeshesIn)
    {
        await _client.EmitAsync("grasshopper:meshes", grasshopperMeshesIn);
    }

    /// <summary>
    /// Disconnects from the web server.
    /// </summary>
    public async Task Disconnect()
    {
        await _client.DisconnectAsync();
    }
}