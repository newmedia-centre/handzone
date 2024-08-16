using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Schema.Socket.Index;
using SocketIOClient;

namespace Handzone.Core
{
    public class RobotConnection
    {
        // private variables
        private readonly SocketIOClient.SocketIO _client;
        private string _status;
        
        // public accessors
        public bool IsConnected { get; private set; }

        public bool IsErrored { get; private set; }
        public RobotInfo Info { get; }

        public string Status => _status;

        internal RobotConnection(JoinSessionOut session)
        {
            Info = session.Robot;
            _client = new SocketIOClient.SocketIO("https://handzone.tudelft.nl/" + session.Robot.Name, new SocketIOOptions()
            {
                Auth = new { token = session.Token }
            });

            _client.OnConnected += (sender, args) =>
            {
                IsConnected = true;
                _status = "Connected to robot";
                
                Console.WriteLine("Connected to robot");
            };
        
            _client.OnDisconnected += (sender, s) =>
            {
                IsConnected = false;
                _status = "Disconnected from robot";
                Console.WriteLine("Disconnected from robot");
            };
        
            _client.OnError += (sender, s) => {
                IsErrored = true;
                _status = s;
                Console.WriteLine($"Error: {s}");
            };
        }

        internal void Connect()
        {
            // try connect to server
            _client.ConnectAsync();
        }
    }
}