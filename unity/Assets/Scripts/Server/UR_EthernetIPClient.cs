/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

#region

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Debug = UnityEngine.Debug;

#endregion

[Serializable]
public class JointValues
{
    public float[] values;
}

[Serializable]
public class TCPValues
{
    public float[] values;
}

[Serializable]
public class GripperStatus
{
    public int status;
}

/// <summary>
/// The UR_EthernetIPClient class manages the communication with a UR robot
/// over Ethernet/IP. It handles the connection, data transmission, and
/// reception of joint values, digital outputs, and other robot-related data.
/// This class also provides methods to control the robot's movements and
/// manage its state.
/// </summary>
public class UR_EthernetIPClient : MonoBehaviour
{
    public AssetReference settings;
    public string urIPAddress = "89.200.111.120";
    public int urReadPort = 30013;
    public int urWritePort = 30003;
    public RobotManager robotManager;
    public float[] readJointValues = new float[JOINT_SIZE];
    public bool digitalOutput = false;
    public float speedScaling;

    private TcpClient _readTcpClient;
    private TcpClient _writeTcpClient;
    private NetworkStream _readStream;
    private NetworkStream _writeStream;
    private bool _isConnected;
    private Thread _readConnectionThread;
    public static bool isMoving = false;
    private Thread _writeConnectionThread;
    private byte[] _readBuffer = new byte[BUFFER_SIZE];
    private byte[] _writeBuffer = new byte[0];
    private Stopwatch readStopwatch = new();
    private Stopwatch writeStopwatch = new();
    private float[] _oldReadValues = new float[JOINT_SIZE];
    private bool _oldDigitalOutput = false;

    private const int BUFFER_SIZE = 11160;
    private const int FIRST_PACKET_SIZE = 4;
    private const byte OFFSET = 8;
    private const uint TOTAL_MSG_LENGTH = 3288596480;
    private const int TIME_STEP = 8;
    private const int JOINT_SIZE = 6;

    public event Action OnConnecting;
    public event Action OnConnected;
    public event Action OnDisconnecting;
    public event Action OnDisconnected;

    public static Action<int, float> JointChanged;
    public static Action<Vector3, Vector3, float, float> UpdateSpeedl;
    public static Action<int, float, float, float> UpdateSpeedj;
    public static Action<float[]> UpdateMovej;
    public static Action<bool> DigitalOutputChanged;
    public static Action ClearSendBuffer;
    public static Action StopMoving;

    /// <summary>
    /// Initializes the UR_EthernetIPClient instance by starting the connection threads
    /// for reading and writing data. It also subscribes to various events for handling
    /// robot commands and logging connection events.
    /// </summary>
    private void Awake()
    {
        _readConnectionThread = new Thread(ConnectToReadAddress);
        _readConnectionThread.IsBackground = true;
        _readConnectionThread.Start();

        _writeConnectionThread = new Thread(ConnectToWriteAddress);
        _writeConnectionThread.IsBackground = true;
        _writeConnectionThread.Start();

        UpdateSpeedl += Speedl;
        UpdateSpeedj += Speedj;
        UpdateMovej += Movej;
        ClearSendBuffer += ClearBuffer;
        StopMoving += StopMoveJ;
        LogEvents();
    }

    /// <summary>
    /// Handles the completion of an asynchronous operation to load the UR IP address
    /// from a settings file. It updates the urIPAddress variable and logs the new address.
    /// </summary>
    /// <param name="obj">The asynchronous operation handle containing the loaded text asset.</param>
    private void Handle_Completed(AsyncOperationHandle<TextAsset> obj)
    {
        urIPAddress = obj.Result.text;
        Debug.Log(urIPAddress);
    }

    /// <summary>
    /// Logs connection events for the UR_EthernetIPClient, including successful
    /// connections and disconnections.
    /// </summary>
    private void LogEvents()
    {
        // OnConnecting += () => Debug.Log("Connecting to Ethernet/IP server...");
        OnConnected += () => Debug.Log("Successfully connected to Ethernet/IP server");
        // OnDisconnecting += () => Debug.Log("Disconnecting from Ethernet/IP server");
        OnDisconnected += () => Debug.Log("Successfully disconnected from Ethernet/IP server");
    }

    /// <summary>
    /// Updates the state of the robot by checking for changes in joint values and
    /// digital outputs. It invokes the corresponding events when changes are detected.
    /// </summary>
    private void Update()
    {
        for (var i = 0; i < readJointValues.Length; i++)
        {
            if (readJointValues[i] != _oldReadValues[i])
            {
                _oldReadValues[i] = readJointValues[i];
                JointChanged?.Invoke(i, readJointValues[i]);
            }

            if (digitalOutput != _oldDigitalOutput)
            {
                _oldDigitalOutput = digitalOutput;
                DigitalOutputChanged?.Invoke(digitalOutput);
                ClearSendBuffer?.Invoke();
            }
        }

        // robotTranslator.UpdateJointsFromPolyscope(readJointValues);
    }

    /// <summary>
    /// Establishes a connection to the robot's read address. It creates a TCP client,
    /// connects to the specified IP address and port, and starts receiving messages
    /// from the robot.
    /// </summary>
    private void ConnectToReadAddress()
    {
        try
        {
            OnConnecting?.Invoke();
            _readTcpClient = new TcpClient();
            _readTcpClient.Connect(IPAddress.Parse(urIPAddress), urReadPort);
            _readStream = _readTcpClient.GetStream();

            _isConnected = true;
            OnConnected?.Invoke();
            ReceiveMessages();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Ethernet/IP connection error: {ex.Message}");
        }
    }

    /// <summary>
    /// Establishes a connection to the robot's write address. It creates a TCP client,
    /// connects to the specified IP address and port, and starts sending messages
    /// to the robot.
    /// </summary>
    private void ConnectToWriteAddress()
    {
        try
        {
            OnConnecting?.Invoke();
            _writeTcpClient = new TcpClient();
            _writeTcpClient.Connect(IPAddress.Parse(urIPAddress), urWritePort);
            _writeStream = _writeTcpClient.GetStream();

            _isConnected = true;
            OnConnected?.Invoke();
            SendMessages();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Ethernet/IP connection error: {ex.Message}");
        }
    }

    /// <summary>
    /// Continuously receives messages from the robot while connected. It reads data
    /// from the network stream and processes the received messages.
    /// </summary>
    private void ReceiveMessages()
    {
        while (_isConnected)
            if (_readStream.Read(_readBuffer, 0, _readBuffer.Length) != 0)
                HandleReadMessage();
        _isConnected = false;
        OnDisconnected?.Invoke();
    }

    /// <summary>
    /// Sends byte messages to the robot over the Ethernet/IP connection. It continuously
    /// writes data to the network stream while connected.
    /// </summary>
    private void SendMessages()
    {
        while (_isConnected) HandleWriteMessage();

        _isConnected = false;
        OnDisconnected?.Invoke();
    }

    /// <summary>
    /// Processes the received messages from the robot. It extracts joint values,
    /// digital outputs, and other relevant data from the received byte array.
    /// </summary>
    private void HandleReadMessage()
    {
        var msgLength = BitConverter.ToUInt32(_readBuffer, FIRST_PACKET_SIZE - 4);
        if (msgLength == TOTAL_MSG_LENGTH)
        {
            readStopwatch.Start();

            Array.Reverse(_readBuffer);

            // Note: Bits values that are referred to can be found in UR Realtime Client Interface.
            var cartesianPosition = new double[3];
            var cartesianOrientation = new double[3];

            var startIndex = _readBuffer.Length - FIRST_PACKET_SIZE;
            var offsetMultiplier = OFFSET;

            // Read actual q values
            for (var i = 0; i < 6; i++)
                readJointValues[i] =
                    (float)BitConverter.ToDouble(_readBuffer, startIndex - (32 + i) * offsetMultiplier);

            // Read actual Tool vector
            for (var i = 0; i < 3; i++)
            {
                cartesianPosition[i] = BitConverter.ToDouble(_readBuffer, startIndex - (56 + i) * offsetMultiplier);
                cartesianOrientation[i] = BitConverter.ToDouble(_readBuffer, startIndex - (59 + i * offsetMultiplier));
            }

            // Read digital outputs
            var digitalOutputValue = BitConverter.ToDouble(_readBuffer, startIndex - 131 * offsetMultiplier);
            digitalOutput = Convert.ToBoolean(digitalOutputValue);

            // Read speed scaling
            speedScaling = (float)BitConverter.ToDouble(_readBuffer, startIndex - 118 * offsetMultiplier);

            readStopwatch.Stop();

            if (readStopwatch.ElapsedMilliseconds < TIME_STEP)
                Thread.Sleep(TIME_STEP - (int)readStopwatch.ElapsedMilliseconds);
            readStopwatch.Restart();
        }
    }

    /// <summary>
    /// Handles the writing of messages to the robot. It sends the current write buffer
    /// to the robot and manages the timing of the write operations.
    /// </summary>
    private void HandleWriteMessage()
    {
        writeStopwatch.Start();

        _writeStream.Write(_writeBuffer, 0, _writeBuffer.Length);

        writeStopwatch.Stop();

        if (writeStopwatch.ElapsedMilliseconds < TIME_STEP)
        {
            if (_writeBuffer.Length > 0)
            {
                // ClearBuffer();
            }

            Thread.Sleep(TIME_STEP - (int)writeStopwatch.ElapsedMilliseconds);
        }

        writeStopwatch.Restart();
    }

    /// <summary>
    /// Sets the write buffer with the provided byte array. This method is used to
    /// prepare data for sending to the robot.
    /// </summary>
    /// <param name="buffer">The byte array to set as the write buffer.</param>
    private void SetWriteBuffer(byte[] buffer)
    {
        _writeBuffer = buffer;
    }

    /// <summary>
    /// Stops the robot's movement by clearing the write buffer and updating the
    /// isMoving state.
    /// </summary>
    private void StopMoveJ()
    {
        ClearBuffer();
        isMoving = false;
    }

    /// <summary>
    /// Clears the write buffer by resetting it to an empty byte array.
    /// </summary>
    private void ClearBuffer()
    {
        _writeBuffer = new byte[0];
    }

    /// <summary>
    /// Sends a linear speed command to the robot based on the provided translation
    /// direction and rotation axis. This method converts the input vectors into
    /// a format suitable for the robot's motion.
    /// </summary>
    /// <param name="translateDirection">The direction of translation as a Vector3.</param>
    /// <param name="rotateAxis">The axis of rotation as a Vector3.</param>
    /// <param name="a">The acceleration for the motion.</param>
    /// <param name="t">The time duration for the motion.</param>
    private void Speedl(Vector3 translateDirection, Vector3 rotateAxis, float a, float t)
    {
        float[] xd =
        {
            -translateDirection.z,
            translateDirection.x,
            translateDirection.y,
            -rotateAxis.z,
            rotateAxis.x,
            rotateAxis.y
        };
        Speedl(xd, a, t);
    }

    /// <summary>
    /// Sets the buffer to send a speedl command to the robot.
    /// </summary>
    /// <param name="xd">Tool speed in m/s (spatial vector)</param>
    /// <param name="a">Tool position acceleration</param>
    /// <param name="t">Time (s) before function returns (optional)</param>
    private void Speedl(float[] xd, float a, float t)
    {
        var commandStr = $"speedl([{xd[0]},{xd[1]},{xd[2]},{xd[3]},{xd[4]},{xd[5]}],a={a},t={t})" + "\n";
        SetWriteBuffer(Encoding.UTF8.GetBytes(commandStr));
    }

    /// <summary>
    /// Sends a joint speed command to the robot for a specific joint.
    /// </summary>
    /// <param name="joint">The index of the joint to control.</param>
    /// <param name="speed">The speed for the joint.</param>
    /// <param name="a">The acceleration for the motion.</param>
    /// <param name="t">The time duration for the motion.</param>
    private void Speedj(int joint, float speed, float a, float t)
    {
        var qd = new float[6];
        for (var i = 0; i < qd.Length; i++)
        {
            qd[i] = 0;
            if (i == joint) qd[i] = speed;
        }

        Speedj(qd, a, t);
    }

    /// <summary>
    /// Sets the buffer to send a speedj command to the robot.
    /// </summary>
    /// <param name="qd">Joint speeds (rad/s)</param>
    /// <param name="a">Joint acceleration (rad/s^2) of leading axis</param>
    /// <param name="t">time in s</param>
    private void Speedj(float[] qd, float a, float t)
    {
        var commandStr = $"speedj([{qd[0]},{qd[1]},{qd[2]},{qd[3]},{qd[4]},{qd[5]}],a={a},t={t})" + "\n";
        SetWriteBuffer(Encoding.UTF8.GetBytes(commandStr));
    }

    /// <summary>
    /// Sets the buffer to send a movej command to the robot.
    /// </summary>
    /// <param name="qd">Joint positions or pose in radians.</param>
    /// <param name="a">Joint acceleration.</param>
    /// <param name="v">Joint speed of leading axis.</param>
    /// <param name="t">Time in seconds.</param>
    /// <param name="r">Blend radius in meters.</param>
    private void Movej(float[] qd, float a = 1.4f, float v = 1.05f, float t = 0f, float r = 0f)
    {
        isMoving = true;

        var commandStr = $"movej([{qd[0]},{qd[1]},{qd[2]},{qd[3]},{qd[4]},{qd[5]}],a={a},v={v},t={t},r={r})" + "\n";
        SetWriteBuffer(Encoding.UTF8.GetBytes(commandStr));

        for (var i = 0; i < qd.Length; i++) qd[i] *= Mathf.Rad2Deg;
    }

    /// <summary>
    /// Sends a movej command to the robot with default parameters.
    /// </summary>
    /// <param name="qd">Joint positions or pose in radians.</param>
    private void Movej(float[] qd)
    {
        Movej(qd, 0.15f, 0.15f, 0f, 0f);
    }

    /// <summary>
    /// Sets a digital output on the robot.
    /// </summary>
    /// <param name="n">The index of the digital output.</param>
    /// <param name="b">The state of the digital output (true or false).</param>
    private void SetDigitalOut(int n, bool b)
    {
        var commandStr = $"set_tool_digital_out({n},{b})" + "\n";
        SetWriteBuffer(Encoding.UTF8.GetBytes(commandStr));
        DigitalOutputChanged?.Invoke(b);
    }

    /// <summary>
    /// Toggles the state of the first digital output on the robot.
    /// </summary>
    public void ToggleDigitalOut()
    {
        SetDigitalOut(0, !digitalOutput);
    }

    /// <summary>
    /// Cleans up the TCP connections and threads when the object is destroyed.
    /// This method ensures that all resources are properly released.
    /// </summary>
    private void OnDestroy()
    {
        try
        {
            OnDisconnecting?.Invoke();
            _isConnected = false;

            if (_readConnectionThread != null && _readConnectionThread.IsAlive)
                _readConnectionThread.Join();

            if (_writeConnectionThread != null && _writeConnectionThread.IsAlive)
                _writeConnectionThread.Join();

            if (_readStream != null)
            {
                _readStream.Close();
                _readStream.Dispose();
            }

            if (_writeStream != null)
            {
                _writeStream.Close();
                _writeStream.Dispose();
            }

            if (_readTcpClient != null) _readTcpClient.Close();

            if (_writeTcpClient != null) _writeTcpClient.Close();

            OnDisconnected?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while cleaning up TCP connection: {e.Message}");
        }
    }
}