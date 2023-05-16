using UnityEngine;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    public string urIPAddress = "192.168.1.1";
    public int urReadPort = 30013;
    public int urWritePort = 30003;
    public RobotTranslator robotTranslator;
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
    public float[] _readJointValues = new float[6];
    
    private const int BUFFER_SIZE = 1116;
    private const int FIRST_PACKET_SIZE = 4;
    private const byte OFFSET = 8;
    private const uint TOTAL_MSG_LENGTH = 3288596480;
    private const int TIME_STEP = 8;
    
    public event Action OnConnect;
    public event Action OnConnecting;
    public event Action OnConnected;
    public event Action OnDisconnect;
    public event Action OnDisconnected;

    public static Action<Vector3, Vector3, float, float> UpdateSpeedl;
    public static Action<float[]> UpdateMovej;
    public static Action ClearSendBuffer;
    public static Action StopMoving;

    void Awake()
    {
        _readConnectionThread = new Thread(ConnectToReadAddress);
        _readConnectionThread.IsBackground = true;
        _readConnectionThread.Start();

        _writeConnectionThread = new Thread(ConnectToWriteAddress);
        _writeConnectionThread.IsBackground = true;
        _writeConnectionThread.Start();

        UpdateSpeedl += Speedl;
        UpdateMovej += Movej;
        ClearSendBuffer += ClearBuffer;
        StopMoving += StopMoveJ;
        LogEvents();
    }

    void LogEvents()
    {
        OnConnect += () => Debug.Log("Connected to Ethernet/IP server");
        OnConnecting += () => Debug.Log("Connecting to Ethernet/IP server...");
        OnConnected += () => Debug.Log("Successfully connected to Ethernet/IP server");
        OnDisconnect += () => Debug.Log("Disconnected from Ethernet/IP server");
        OnDisconnected += () => Debug.Log("Successfully disconnected from Ethernet/IP server");
    }

    private void Update()
    {
        robotTranslator.UpdateJoints(_readJointValues);
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
            OnConnect?.Invoke();
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
            OnConnect?.Invoke();
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

            for (int i = 0; i < 6; i++)
            {
                _readJointValues[i] = (float)BitConverter.ToDouble(_readBuffer, startIndex - ((32 + i) * offsetMultiplier));
            }
            
            for (int i = 0; i < 3; i++)
            {
                cartesianPosition[i] = BitConverter.ToDouble(_readBuffer, startIndex - ((56 + i) * offsetMultiplier));
                cartesianOrientation[i] = BitConverter.ToDouble(_readBuffer, startIndex - (59 + i * offsetMultiplier));
            }
            
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
            Thread.Sleep(TIME_STEP - (int)writeStopwatch.ElapsedMilliseconds);
        }
        writeStopwatch.Restart();
    }

    void SetWriteBuffer(byte[] buffer)
    {
        _writeBuffer = buffer;
        Debug.Log("New Write buffer: " + _writeBuffer.Length);
    }

    void StopMoveJ()
    {
        ClearBuffer();
        isMoving = false;
    }
    
    private void ClearBuffer()
    {
        Debug.Log("Clearing buffer");
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
        Debug.Log($"Angles: {qd[0]},{qd[1]},{qd[2]},{qd[3]},{qd[4]},{qd[5]}");
        Debug.Log(commandStr);
    }

    void Movej(float[] qd)
    {
        Movej(qd, 0.3f, 0.3f, 0f, 0f);
    }

    void SetDigitalOut(int n, bool b)
    {
        string commandStr = $"set_digital_out({n},{b})" + "\n";
        SetWriteBuffer(Encoding.UTF8.GetBytes(commandStr));
    }

    void OnDestroy()
    {
        _readStream.Close();
        _readTcpClient.Close();
        _readStream.Dispose();
        _readTcpClient.Dispose();
        
        _writeStream.Close();
        _writeTcpClient.Close();
        _writeStream.Dispose();
        _writeTcpClient.Dispose();
        
        Thread.Sleep(100);
        _readConnectionThread.Abort();
        _writeConnectionThread.Abort();
    }
}