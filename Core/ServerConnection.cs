using System;
using System.Threading;

namespace Handzone.Core
{
    public class ServerConnection
    {
        // private variables
        private SocketIOClient.SocketIO _client;
        private bool _isConnected;
        private bool _isErrored;
        private string _status;
        
        // public accessors
        public bool IsConnected => _isConnected;
        public bool IsErrored => _isErrored;
        public string Status => _status;

        internal ServerConnection()
        {
            _client = new SocketIOClient.SocketIO("https://handzone.tudelft.nl");

            _client.OnConnected += (sender, args) =>
            {
                _isConnected = true;
                _status = "Connected to server";
                Console.WriteLine("Connected to server");
            };
        
            _client.OnDisconnected += (sender, s) =>
            {
                _isConnected = false;
                _status = "Disconnected from server";
                Console.WriteLine("Disconnected from server");
            };
        
            _client.OnError += (sender, s) => {
                // check if more time is needed
                if (s == "Pin not claimed")
                {
                    Console.WriteLine("Pin not claimed, waiting and retrying...");
                    Thread.Sleep(1000);
                    _client.ConnectAsync();
                }
                else
                {
                    _isErrored = true;
                    _status = s;
                    Console.WriteLine($"Error: {s}");
                }
            };
        }

        internal void Connect(string pin)
        {
            // set auth object
            _client.Options.Auth = new
            {
                pin,
                signature = State.Signature
            };
            
            // try connect to server
            _client.ConnectAsync();
        }
    }
}