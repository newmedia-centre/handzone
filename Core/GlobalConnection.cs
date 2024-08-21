using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
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
    
    public JoinSessionOut Session { get; private set; }
    public bool Connected = false;
    
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

        _client.OnConnected += (sender, args) =>
        {
            Connected = true;
            OnStatus?.Invoke("Connected to global server");
            OnConnectionChange?.Invoke(true);
        };

        _client.OnDisconnected += (sender, s) =>
        {
            Connected = false;
            OnStatus?.Invoke("Disconnected from global server");
            OnConnectionChange?.Invoke(false);
        };

        _client.OnError += (sender, s) =>
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

    public async Task Disconnect()
    {
        await _client.DisconnectAsync();
    }
}