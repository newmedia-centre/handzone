using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Schema.Socket.Index;
using SocketIO.Serializer.NewtonsoftJson;
using SocketIOClient;

namespace Handzone.Core;

public class GlobalConnection
{
    /// <summary>
    /// Socket.io client
    /// </summary>
    private SocketIOClient.SocketIO _client;
    
    public bool Connected;
    
    // Connection lifecycle events
    public event Action<string> OnError;
    public event Action<string> OnStatus;
    public event Action<bool> OnConnectionChange;
    
    /// <summary>
    /// Attempts to connect to the web server using the provided PIN.
    /// Registers connection, disconnection, and error handling events.
    /// </summary>
    /// <param name="pin">The PIN used for secure connection.</param>
    public async Task TryConnectToGlobalServer(string pin)
    {
        Connected = false;
        OnStatus?.Invoke("Connecting to global server...");
        
        _client = new SocketIOClient.SocketIO(State.Url, new SocketIOOptions
        {
            Auth = new
            {
                pin,
                signature = State.Signature
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
            OnStatus?.Invoke("Connected to global server");
            OnConnectionChange?.Invoke(true);
        };

        _client.OnDisconnected += (_, _) =>
        {
            Connected = false;
            OnStatus?.Invoke("Disconnected from global server");
            OnConnectionChange?.Invoke(false);
        };

        _client.OnError += (_, s) =>
        {
            Console.WriteLine(s);
            
            // check if more time is needed
            if (s == "Pin not claimed")
            {
                OnStatus?.Invoke("Pin not claimed, waiting and retrying...");
                Thread.Sleep(1000);
                _client.ConnectAsync();
            }
            else
            {
                OnError?.Invoke(s);
                OnConnectionChange?.Invoke(false);
            }
        };
        
        Console.WriteLine("Pre Connect");

        await _client.ConnectAsync();
        
        Console.WriteLine("Post Connect");
    }

    /// <summary>
    /// Gets the namespace the user is already using.
    /// </summary>
    public async Task<JoinSessionOut> GetNamespace()
    {
        var tcs = new TaskCompletionSource<(JoinSessionOut, string)>();
        
        await _client.EmitAsync("namespace", response =>
        {
            var success = response.GetValue<bool>(0);
            if (success)
            {
                tcs.SetResult((response.GetValue<JoinSessionOut>(1), null));
            }
            else
            {
                tcs.SetResult((null, response.GetValue<string>(1)));
            }
        });

        var result = await tcs.Task;
        if (result.Item1 == null)
        {
            throw new Exception(result.Item2);
        }

        return result.Item1;
    }

    /// <summary>
    /// Disconnects from the web server.
    /// </summary>
    public async Task Disconnect()
    {
        await _client.DisconnectAsync();
    }
}