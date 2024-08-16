using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Schema.Socket.Index;

namespace Handzone.Core
{
    public class ServerConnection
    {
        // private variables
        private readonly SocketIOClient.SocketIO _client;
        private string _status;
        
        // public accessors
        public bool IsConnected { get; private set; }

        public bool IsErrored { get; private set; }
        
        public List<RobotSession> Sessions { get; private set; }

        public string Status => _status;

        internal ServerConnection()
        {
            _client = new SocketIOClient.SocketIO("https://handzone.tudelft.nl");
            Sessions = new List<RobotSession>();

            _client.OnConnected += (sender, args) =>
            {
                IsConnected = true;
                _status = "Connected to server";
                Console.WriteLine("Connected to server");
            };
        
            _client.OnDisconnected += (sender, s) =>
            {
                IsConnected = false;
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
                    IsErrored = true;
                    _status = s;
                    Console.WriteLine($"Error: {s}");
                }
            };
            
            _client.On("sessions", response =>
            {
                var sessions = response.GetValue<SessionsOut>();
                if (sessions == null)
                {
                    Sessions = new List<RobotSession>();
                }
                else
                {
                    Sessions = sessions.Sessions;
                }
            });
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

        internal JoinSessionOut GetActiveSession()
        {
            var tcs = new TaskCompletionSource<(JoinSessionOut, string)>();
            
            _client.EmitAsync("namespace", response =>
            {
                var success = response.GetValue<bool>();
                if (success)
                {
                    tcs.SetResult((response.GetValue<JoinSessionOut>(1), null));
                }
                else
                {
                    tcs.SetResult((null, response.GetValue<string>(1)));
                }
            });
            
            var result = tcs.Task.GetAwaiter().GetResult();
            if (result.Item1 == null)
            {
                throw new Exception(result.Item2);
            }

            return result.Item1;
        }
    }
}