using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using Grasshopper.Kernel;

namespace Handzone.Core
{
    public class State
    {
        // define state singletons
        private static string _signature;
        private static SettingsServer _settingsServer;

        // Room for other instaces, e.g. FooServer or BarServer

        // Private constructor.
        private State()
        {}
        
        // Signature accessor that allows only one instance.
        public static string Signature => _signature ?? (_signature = NewSignature());

        // Settings server accessor that allows only one instance.
        public static SettingsServer SettingsServer
        {
            get
            {
                if (_settingsServer == null)
                    _settingsServer = new SettingsServer();

                return _settingsServer;
            }
        }
        
        // generate a secure signature to identify the auth flow with
        public static string NewSignature()
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

    public class SettingsServer
    {
        // Constructor
        public SettingsServer(){}
    }
}