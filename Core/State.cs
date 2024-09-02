/*
 * Copyright 2024 NewMedia Centre - Delft University of Technology
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
        public const string Url = "https://handzone.tudelft.nl/";

        // Private constructor.
        private State()
        { }

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