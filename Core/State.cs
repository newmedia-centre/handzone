using System;
using System.Security.Cryptography;
using Schema.Socket.Index;

namespace Handzone.Core
{
    public class State
    {
        // define state singletons
        private static string _signature;
        private static ServerConnection _serverConnection;
        private static RobotConnection _robotConnection;
        
        // define constants
        public const string URL = "http://localhost:3000/";

        // Private constructor.
        private State()
        {}
        
        // Signature accessor that allows only one instance.
        public static string Signature => _signature ?? (_signature = NewSignature());

        // ServerConnection accessor that allows only one instance.
        public static ServerConnection ServerConnection
        {
            get
            {
                if (_serverConnection == null)
                    _serverConnection = new ServerConnection();

                return _serverConnection;
            }
        }
        
        // RobotConnection accessor that allows only one instance.
        public static RobotConnection RobotConnection => _robotConnection;

        // generate a secure signature to identify the auth flow with
        internal static string NewSignature()
        {
            byte[] signature = new byte[32];
            
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(signature);
            }
            
            _signature = Convert.ToBase64String(signature);
            return _signature;
        }
        
        // create a new RobotConnection
        internal static RobotConnection NewRobotConnection(JoinSessionOut session)
        {
            _robotConnection = new RobotConnection(session);
            return _robotConnection;
        }
    }
}