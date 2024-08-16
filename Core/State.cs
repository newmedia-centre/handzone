using System;
using System.Security.Cryptography;

namespace Handzone.Core
{
    public class State
    {
        // define state singletons
        private static string _signature;
        private static ServerConnection _serverConnection;

        // Private constructor.
        private State()
        {}
        
        // Signature accessor that allows only one instance.
        public static string Signature => _signature ?? (_signature = NewSignature());

        // Settings server accessor that allows only one instance.
        public static ServerConnection ServerConnection
        {
            get
            {
                if (_serverConnection == null)
                    _serverConnection = new ServerConnection();

                return _serverConnection;
            }
        }
        
        // generate a secure signature to identify the auth flow with
        internal static string NewSignature()
        {
            byte[] signature = new byte[32];
            
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(signature);
            }
            
            string encoded = Convert.ToBase64String(signature);
            _signature = encoded;
            return encoded;
        }
    }
}