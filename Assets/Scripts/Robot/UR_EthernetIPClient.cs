using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

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

public class UR_EthernetIPClient : MonoBehaviour
{
    public AssetReference settings;
    public string urIPAddress = "192.168.1.1";
    public int urReadPort = 30013;
    public int urWritePort = 30003;
    public RobotTranslator robotTranslator;
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
    
    private const int BUFFER_SIZE = 1116;
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

    void Awake()
    {
        var handle = settings.LoadAssetAsync<TextAsset>();
        handle.Completed += Handle_Completed;
        
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

    private void Handle_Completed(AsyncOperationHandle<TextAsset> obj)
    {
        urIPAddress = obj.Result.text;
        Debug.Log(urIPAddress);
    }

    void LogEvents()
    {
        // OnConnecting += () => Debug.Log("Connecting to Ethernet/IP server...");
        OnConnected += () => Debug.Log("Successfully connected to Ethernet/IP server");
        // OnDisconnecting += () => Debug.Log("Disconnecting from Ethernet/IP server");
        OnDisconnected += () => Debug.Log("Successfully disconnected from Ethernet/IP server");
    }

    private void Update()
    {
        for (int i = 0; i < readJointValues.Length; i++)
        {
            if(readJointValues[i] != _oldReadValues[i])
            {
                _oldReadValues[i] = readJointValues[i];
                JointChanged?.Invoke(i, readJointValues[i]);
            }
            
            if(digitalOutput != _oldDigitalOutput)
            {
                _oldDigitalOutput = digitalOutput;
                DigitalOutputChanged?.Invoke(digitalOutput);
                ClearSendBuffer?.Invoke();
            }
        }
        
        robotTranslator.UpdateJointsFromPolyscope(readJointValues);
    }

    void ConnectToReadAddress()
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
            Debug.LogError($"Ethernet/IP connection error: {ex.Message}");
        }
    }

    void ConnectToWriteAddress()
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
            Debug.LogError($"Ethernet/IP connection error: {ex.Message}");
        }
    }

    void ReceiveMessages()
    {
        while (_isConnected)
        {
            if(_readStream.Read(_readBuffer, 0, _readBuffer.Length) != 0)
            {
                HandleReadMessage();
            }
        }
        _isConnected = false;
        OnDisconnected?.Invoke();
    }
    
    // Sends byte message to Ethernet/IP server
    void SendMessages()
    {
        while (_isConnected)
        {
            HandleWriteMessage();
        }

        _isConnected = false;
        OnDisconnected?.Invoke();
    }

    void HandleReadMessage()
    {
        uint msgLength = BitConverter.ToUInt32(_readBuffer, FIRST_PACKET_SIZE - 4);
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
            for (int i = 0; i < 6; i++)
            {
                readJointValues[i] = (float)BitConverter.ToDouble(_readBuffer, startIndex - ((32 + i) * offsetMultiplier));
            }
            
            // Read actual Tool vector
            for (int i = 0; i < 3; i++)
            {
                cartesianPosition[i] = BitConverter.ToDouble(_readBuffer, startIndex - ((56 + i) * offsetMultiplier));
                cartesianOrientation[i] = BitConverter.ToDouble(_readBuffer, startIndex - (59 + i * offsetMultiplier));
            }
            
            // Read digital outputs
            double digitalOutputValue = BitConverter.ToDouble(_readBuffer, startIndex - (131 * offsetMultiplier));
            digitalOutput = Convert.ToBoolean(digitalOutputValue);
            
            // Read speed scaling
            speedScaling = (float)BitConverter.ToDouble(_readBuffer, startIndex - (118 * offsetMultiplier));
            
            readStopwatch.Stop();
            
            if(readStopwatch.ElapsedMilliseconds < TIME_STEP)
            {
                Thread.Sleep(TIME_STEP - (int)readStopwatch.ElapsedMilliseconds);
            }
            readStopwatch.Restart();
        }
    }

    void HandleWriteMessage()
    {
        writeStopwatch.Start();

        _writeStream.Write(_writeBuffer, 0, _writeBuffer.Length);
        
        writeStopwatch.Stop();
        
        if(writeStopwatch.ElapsedMilliseconds < TIME_STEP)
        {
            if (_writeBuffer.Length > 0)
            {
                // ClearBuffer();
            }
            Thread.Sleep(TIME_STEP - (int)writeStopwatch.ElapsedMilliseconds);
        }
        writeStopwatch.Restart();
    }

    void SetWriteBuffer(byte[] buffer)
    {
        _writeBuffer = buffer;
    }

    void StopMoveJ()
    {
        ClearBuffer();
        isMoving = false;
    }
    
    private void ClearBuffer()
    {
        _writeBuffer = new byte[0];
    }

    void Speedl(Vector3 translateDirection, Vector3 rotateAxis, float a, float t)
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
    void Speedl(float[] xd, float a, float t)
    {
        string commandStr = $"speedl([{xd[0]},{xd[1]},{xd[2]},{xd[3]},{xd[4]},{xd[5]}],a={a},t={t})" + "\n";
        SetWriteBuffer(Encoding.UTF8.GetBytes(commandStr));
    }

    private void Speedj(int joint, float speed, float a, float t)
    {
        float[] qd = new float[6];
        for (int i = 0; i < qd.Length; i++)
        {
            qd[i] = 0;
            if (i == joint)
            {
                qd[i] = speed;
            }
        }
        
        Speedj(qd, a, t);
    }
    
    /// <summary>
    /// Sets the buffer to send a speedj command to the robot.
    /// </summary>
    /// <param name="qd">Joint speeds (rad/s)</param>
    /// <param name="a">Joint acceleration (rad/s^2) of leading axis</param>
    /// <param name="t">time in s</param>
    void Speedj(float[] qd, float a, float t)
    {
        string commandStr = $"speedj([{qd[0]},{qd[1]},{qd[2]},{qd[3]},{qd[4]},{qd[5]}],a={a},t={t})" + "\n";
        SetWriteBuffer(Encoding.UTF8.GetBytes(commandStr));
    }
    
    /// <summary>
    /// Sets the buffer to send a movej command to the robot.
    /// </summary>
    /// <param name="qd">Joint positions or pose in rads</param>
    /// <param name="a">Joint acceleration</param>
    /// <param name="v">Joint speed of leading axis</param>
    /// <param name="t">Time in seconds</param>
    /// <param name="r">Blend radius in meters</param>
    void Movej(float[] qd, float a = 1.4f, float v = 1.05f, float t = 0f, float r = 0f)
    {
        isMoving = true;
        
        string commandStr = $"movej([{qd[0]},{qd[1]},{qd[2]},{qd[3]},{qd[4]},{qd[5]}],a={a},v={v},t={t},r={r})" + "\n";
        SetWriteBuffer(Encoding.UTF8.GetBytes(commandStr));
        
        for (int i = 0; i < qd.Length; i++)
        {
            qd[i] *= Mathf.Rad2Deg;
        }
    }

    void Movej(float[] qd)
    {
        Movej(qd, 0.15f, 0.15f, 0f, 0f);
    }

    void SetDigitalOut(int n, bool b)
    {
        string commandStr = $"set_tool_digital_out({n},{b})" + "\n";
        SetWriteBuffer(Encoding.UTF8.GetBytes(commandStr));
        DigitalOutputChanged?.Invoke(b);
    }
    
    public void ToggleDigitalOut()
    {
        SetDigitalOut(0, !digitalOutput);
    }
    
    void OnDestroy()
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
            
            if(_writeStream != null)
            {
                _writeStream.Close();
                _writeStream.Dispose();
            }

            if (_readTcpClient != null)
            {
                _readTcpClient.Close();
            }
            
            if(_writeTcpClient != null)
            {
                _writeTcpClient.Close();
            }
            
            OnDisconnected?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while cleaning up TCP connection: {e.Message}");
        }
    }
}