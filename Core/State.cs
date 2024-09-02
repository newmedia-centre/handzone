using System;
using System.Security.Cryptography;

namespace Handzone.Core
{
    public class State
    {
        // define state singletons
        private static string _signature;
        private static GlobalConnection _globalConnection;
        private static SessionConnection _sessionConnection;
        
        // define constants
        public const string Url = "http://172.19.14.253:3000/";

        // Private constructor.
        private State()
        {}
        
        // Signature accessor that allows only one instance.
        public static string Signature => _signature ?? NewSignature();
        
        // GlobalConnection accessor that allows only one instance.
        public static GlobalConnection GlobalConnection
        {
            get
            {
                if (_globalConnection == null)
                    _globalConnection = new GlobalConnection();

                return _globalConnection;
            }
        }
        
        // SessionConnection accessor that allows only one instance.
        public static SessionConnection SessionConnection
        {
            get
            {
                if (_sessionConnection == null)
                    _sessionConnection = new SessionConnection();

                return _sessionConnection;
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
            
            _signature = Convert.ToBase64String(signature);
            return _signature;
        }
    }
}