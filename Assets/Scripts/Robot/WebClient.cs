using System;
using System.Threading.Tasks;
using UnityEngine;
using SocketIO.Serializer.NewtonsoftJson;

public class WebClient : MonoBehaviour
{
    [SerializeField] private string _url = "http://172.19.14.253:3000/172.19.14.251";
    private SocketIOClient.SocketIO _client;
    private string[] _robots;

    private WebClient()
    {
        _client = new SocketIOClient.SocketIO(_url);
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
        _client.On("realtime:data", response =>
        {
            // You can print the returned data first to decide what to do next.
            // output: ["hi client"]

            RealtimeData data = response.GetValue<RealtimeData>();
            Debug.Log(data.MessageSize);

        });

        _client.OnConnected += async (sender, e) =>
        {
            // Emit a movej command to the server
            double[] q = new[] { 0, 1.57, -1.57, 3.14, -1.57, 1.57 };
            await _client.EmitAsync("motion:movej", q, 1, 1, 0, 0);
        };
        await _client.ConnectAsync();
    }
}
