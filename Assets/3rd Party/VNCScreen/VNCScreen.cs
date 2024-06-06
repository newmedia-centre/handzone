// Unity 3D Vnc Client - Unity 3D VNC Client Library
// Copyright (C) 2017 Christophe Floutier
//
// Based on VncSharp - .NET VNC Client Library
// Copyright (C) 2008 David Humphrey
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityVncSharp;
using VNCScreen.Drawing;

namespace VNCScreen
{
    /// <summary>
    /// SpecialKeys is a list of the various keyboard combinations that overlap with the client-side and make it
    /// difficult to send remotely.  These values are used in conjunction with the SendSpecialKeys method.
    /// </summary>
    public enum SpecialKeys
    {
        CtrlAltDel,
        AltF4,
        CtrlEsc,
        Ctrl,
        Alt
    }


    /// <summary>
    /// The Main Component. 
    /// It must be placed on a Quad Mesh with a MeshCollider
    /// The password, host, port and display must be configured in the editor fields
    /// 
    /// To control the screen, use a VNCMouseRaycaster for instance.
    /// 
    /// This code is mainly adapted from RemoteDesktop code from the VncSharp
    /// </summary>
    public class VNCScreen : MonoBehaviour
    {
        public string host;
        public int port = 5900;
        public int display;
        public string password;
        public Material disconnectedScreen;
        public Material connectedMaterial;
        public Vector2 mousePosition;

        private Size _screenSize;
        private bool _passwordPending;            // After Connect() is called, a password might be required.
        private bool _fullScreenRefresh;		     // Whether or not to request the entire remote screen be sent.
        private Material _m;
        private Thread _mainThread;
        private bool _secure = false;
        
        public bool Secure => _secure;
        public Size ScreenSize => _screenSize;

        // Use this for initialization
        void Start()
        {
            _mainThread = Thread.CurrentThread;

            SetDisconnectedMaterial();

            if (SessionClient.Instance == null)
            {
                Debug.LogWarning("SessionClient instance is null. Make sure to have a SessionClient instance in the scene.");
                return;
            }
            
            SessionClient.Instance.OnConnected += Connect;
            Connect();
        }

        void SetDisconnectedMaterial()
        {
            if (disconnectedScreen != null)
            {
                GetComponent<Renderer>().sharedMaterial = disconnectedScreen;
            }
        }

        private IVncClient _vnc;                           // The Client object handling all protocol-level interaction

        public enum RuntimeState
        {
            Disconnected,
            Disconnecting,

            Connecting,
            WaitFirstBuffer,
            Connected,
            Error
        }

        public enum VncPlugin
        {
            VNCSharp,
            RealVnc
        }

        public VncPlugin plugin;

        IVncClient BuildVnc()
        {
            switch (plugin)
            {
                default:
                case VncPlugin.VNCSharp:
                    return new VNCSharpClient();
                case VncPlugin.RealVnc:
                    {
                        return gameObject.AddComponent<RealVncClient>();
                    }
            }
        }

        public RuntimeState state = RuntimeState.Disconnected;
        public delegate void OnStateChanged(RuntimeState state);
        public event OnStateChanged OnStateChangedEvent;

        /// <summary>
        /// True if the RemoteDesktop is connected and authenticated (if necessary) with a remote VNC Host; otherwise False.
        /// </summary>
        public bool IsConnected => state == RuntimeState.Connected || state == RuntimeState.WaitFirstBuffer;

        public void Connect()
        {
            // Ignore attempts to use invalid port numbers
            if (port < 1 | port > 65535) port = 5900;
            if (display < 0) display = 0;
            if (host == null) throw new ArgumentNullException("host");

            StartCoroutine(Connection());
        }

        IEnumerator Connection()
        {
            if (IsConnected)
            {
                Disconnect();
            }

            while (state != RuntimeState.Disconnected)
            {
                yield return new WaitForEndOfFrame();
            }

            // Start protocol-level handling and determine whether a password is needed
            _vnc = BuildVnc();
            _vnc.ConnectionLost += OnConnectionLost;
            _vnc.onConnection += Vnc_onConnection;
            _connectionReceived = false;
            SetState(RuntimeState.Connecting);

            _vnc.Connect(host, display, port, false);

            Debug.Log("[VNCScreen] Connection In progress " + host + ":" + port);

            while (!_connectionReceived)
                yield return new WaitForFixedUpdate();

            if (_needPassword)
            {
                // Server needs a password, so call which ever method is refered to by the GetPassword delegate.
                if (string.IsNullOrEmpty(password))
                {
                    // No password could be obtained (e.g., user clicked Cancel), so stop connecting
                    SetState(RuntimeState.Error);
                }
                else
                {
                    Authenticate(password);
                }
            }
            else
            {
                // No password needed, so go ahead and Initialize here
                Initialize();
            }
        }

        private bool _connectionReceived;
        private bool _needPassword;

        private void Vnc_onConnection(Exception errorConnection, bool needPassword)
        {
            if (errorConnection != null)
            {
                OnConnectionLost(this, new ErrorEventArg(errorConnection));
                return;
            }
            _connectionReceived = true;
            _needPassword = needPassword;
        }

        /// <summary>
        /// Authenticate with the VNC Host using a user supplied password.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is already Connected.  See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>
        /// <exception cref="System.NullReferenceException">Thrown if the password is null.</exception>
        /// <param name="password">The user's password.</param>
        private void Authenticate(string password)
        {
            if (!_passwordPending)
                throw new InvalidOperationException("Authentication is only required when Connect() returns True and the VNC Host requires a password.");

            if (password == null)
                throw new NullReferenceException("password");

            _passwordPending = false;  // repeated calls to Authenticate should fail.
            _vnc.Authenticate(password, OnPassword);
        }

        void OnPassword(bool ok)
        {
            if (ok)
            {
                Initialize();
            }
            else
            {
                OnConnectionLost("Wrong Password");
            }
        }

        void OnConnectionLost(string reason)
        {
            OnConnectionLost(this, new ErrorEventArg(reason));
        }


        /// <summary>
        /// received when a connection lost has been detected, the disconnect function will also call this fucntion
        /// </summary>
        /// <param name="sender">The VncClient object that raised the event.</param>
        /// <param name="e">An empty EventArgs object.</param>
        private void OnConnectionLost(object sender, EventArgs e)
        {
            // If the remote host dies, and there are attempts to write
            // keyboard/mouse/update notifications, this may get called 
            // many times, and from main or worker thread.
            // Guard against this and invoke Disconnect once.
            if (state == RuntimeState.Connected)
            {
                state = RuntimeState.Disconnecting;

                Disconnect();
            }

            if (e is ErrorEventArg)
            {
                var error = e as ErrorEventArg;

                if (error.Exception != null)
                {
                    SetState(RuntimeState.Error);
                    Debug.LogException(error.Exception);
                }
                else if (!string.IsNullOrEmpty(error.Reason))
                {
                    Debug.Log("[VNCScreen]" + error.Reason); 
                }
            }
            else
            {
                Debug.Log("VncDesktop_ConnectionLost");
            }

            // If the remote host dies, try to request a new token and reconnect 
            if (GlobalClient.Instance)
            {
                Task.Run(async () => await GlobalClient.Instance.RequestToken());
                Connect();
            }
        }

        private List<RuntimeState> _stateChanges = new();

        private void SetState(RuntimeState newState)
        {
            var isMainThread = _mainThread.Equals(Thread.CurrentThread);
            if (!isMainThread)
            {
                _stateChanges.Add(newState);
                return;
            }


            switch (newState)
            {
                case RuntimeState.Disconnected:
                    SetDisconnectedMaterial();
                    break;
                case RuntimeState.Disconnecting:
                    SetDisconnectedMaterial();
                    break;
                case RuntimeState.Connected:
                    break;
                case RuntimeState.WaitFirstBuffer:
                    SetDisconnectedMaterial();
                    break;
                case RuntimeState.Connecting:
                    SetDisconnectedMaterial();
                    break;
                case RuntimeState.Error:
                    SetDisconnectedMaterial();
                    break;
            }
            state = newState;

            if (OnStateChangedEvent != null)
                OnStateChangedEvent(state);
        }

        /// <summary>
        /// Get a complete update of the entire screen from the remote host.
        /// </summary>
        /// <remarks>You should allow users to call FullScreenUpdate in order to correct
        /// corruption of the local image.  This will simply request that the next update be
        /// for the full screen, and not a portion of it.  It will not do the update while
        /// blocking.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is not in the Connected state.  See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>
        public void FullScreenUpdate()
        {
            if (state != RuntimeState.Connected) return;

            _fullScreenRefresh = true;
        }

        /// <summary>
        /// After protocol-level initialization and connecting is complete, the local GUI objects have to be set-up, and requests for updates to the remote host begun.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is already in the Connected state.  See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>		
        protected bool Initialize()
        {
            // Finish protocol handshake with host now that authentication is done.  
            _vnc.Initialize();
            SetState(RuntimeState.WaitFirstBuffer);

            _screenSize = _vnc.BufferSize;

            // Tell the user of this control the necessary info about the desktop in order to setup the display
            // Create a texture
            Texture2D tex = _vnc.getTexture();

            if (connectedMaterial == null)
                connectedMaterial = GetComponent<Renderer>().sharedMaterial;

            // Set texture onto our material
            _m = Instantiate(connectedMaterial);
            _m.mainTexture = tex;
            GetComponent<Renderer>().sharedMaterial = _m;

            // Refresh scroll properties
            //AutoScrollMinSize = desktopPolicy.AutoScrollMinSize;

            // Start getting updates from the remote host (vnc.StartUpdates will begin a worker thread).          
            _vnc.StartUpdates();

            return true;
        }

        public void Disconnect()
        {
            _vnc.ConnectionLost -= OnConnectionLost;

            _vnc.Disconnect();
            SetState(RuntimeState.Disconnected);
            OnConnectionLost("Disconnected");
        }


        // Update is called once per frame
        void Update()
        {
            if (IsConnected)
            {
                GetComponent<Renderer>().sharedMaterial.mainTexture = _vnc.getTexture();

                if (_vnc.updateDesktopImage())
                {
                    if (state == RuntimeState.WaitFirstBuffer)
                        SetState(RuntimeState.Connected);
                }

                if (state == RuntimeState.Connected)
                {
                    _vnc.RequestScreenUpdate(_fullScreenRefresh);

                    // Make sure the next screen update is incremental
                    _fullScreenRefresh = false;
                }

            }

            for (int i = 0; i < _stateChanges.Count; i++)
            {
                SetState(_stateChanges[i]);
            }

            _stateChanges.Clear();
        }

        void OnApplicationQuit()
        {
            if (IsConnected)
                Disconnect();
        }

        public void UpdateMouse(Vector2 pos, bool button0, bool button1, bool button2)
        {
            if (!IsConnected) return;

            Size s = _vnc.BufferSize;
            Point point = new Point((int)(pos.x * s.Width), (int)(pos.y * s.Height));

            UpdateMouse(point, button0, button1, button2);
        }

        public void UpdateMouse(Point pos, bool button0, bool button1, bool button2)
        {
            if (!IsConnected) return;
            
            mousePosition = new Vector2(pos.X, pos.Y);
            Debug.Log(mousePosition + " VNC Mouse position");
            _vnc.UpdateMouse(pos, button0, button1, button2);
        }

        public void UpdateMouse(Vector2 pos, bool button0)
        {
            if (!IsConnected) return;

            Size s = _vnc.BufferSize;
            Point point = new Point((int)(pos.x * s.Width), (int)(pos.y * s.Height));

            UpdateMouse(point, button0, false, false);
        }
        
        /// <summary>
        /// Sends a keyboard combination that would otherwise be reserved for the client PC.
        /// </summary>
        /// <param name="keys">SpecialKeys is an enumerated list of supported keyboard combinations.</param>
        /// <remarks>Keyboard combinations are Pressed and then Released, while single keys (e.g., SpecialKeys.Ctrl) are only pressed so that subsequent keys will be modified.</remarks>
        /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is not in the Connected state.</exception>
        public void SendSpecialKeys(SpecialKeys keys)
        {
            SendSpecialKeys(keys, true);
        }

        /// <summary>
        /// Sends a keyboard combination that would otherwise be reserved for the client PC.
        /// </summary>
        /// <param name="keys">SpecialKeys is an enumerated list of supported keyboard combinations.</param>
        /// <remarks>Keyboard combinations are Pressed and then Released, while single keys (e.g., SpecialKeys.Ctrl) are only pressed so that subsequent keys will be modified.</remarks>
        /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is not in the Connected state.</exception>
        public void SendSpecialKeys(SpecialKeys keys, bool release)
        {
            if (state != RuntimeState.Connected) return;

            // For all of these I am sending the key presses manually instead of calling
            // the keyboard event handlers, as I don't want to propegate the calls up to the 
            // base control class and form.
            switch (keys)
            {
                case SpecialKeys.Ctrl:
                    PressKeys(new uint[] { 0xffe3 }, true, release);  // CTRL, but don't release
                    break;
                case SpecialKeys.Alt:
                    PressKeys(new uint[] { 0xffe9 }, true, release);  // ALT, but don't release
                    break;
                case SpecialKeys.CtrlAltDel:
                    PressKeys(new uint[] { 0xffe3, 0xffe9, 0xffff }, true, release); // CTRL, ALT, DEL
                    break;
                case SpecialKeys.AltF4:
                    PressKeys(new uint[] { 0xffe9, 0xffc1 }, true, release); // ALT, F4
                    break;
                case SpecialKeys.CtrlEsc:
                    PressKeys(new uint[] { 0xffe3, 0xff1b }, true, release); // CTRL, ESC
                    break;
                // TODO: are there more I should support???
            }
        }

        /// <summary>
        /// Given a list of keysym values, sends a key press for each, then a release.
        /// </summary>
        /// <param name="keys">An array of keysym values representing keys to press/release.</param>
        /// <param name="release">A boolean indicating whether the keys should be Pressed and then Released.</param>
        private void PressKeys(uint[] keys, bool pressed, bool released)
        {
            //        System.Diagnostics.Debug.Assert(keys != null, "keys[] cannot be null.");

            for (int i = 0; i < keys.Length; ++i)
            {
                PressKey(keys[i], pressed, released);
            }
        }

        private void PressKey(uint key, bool pressed, bool released)
        {
            if (IsConnected)
            {
                //    Debug.Log("Press Key " + key + " - " + pressed + " - " + released);

                if (pressed)
                    _vnc.WriteKeyboardEvent(key, true);
                if (released)
                    _vnc.WriteKeyboardEvent(key, false);
            }
        }

        public void OnKey(KeyCode key, bool pressed)
        {
            uint code = KeyTranslator.convertToXKCode(key);
            PressKey(code, pressed, !pressed);
        }
    }
}
