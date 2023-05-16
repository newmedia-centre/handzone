using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json;

/// <summary>
/// This code is a Unity script that establishes an MQTT (Message Queue Telemetry Transport) communication with a
/// Universal Robots (UR) robot using the M2MqttUnity library.
/// It receives messages with robot joint positions and updates the joint positions in Unity accordingly.
/// </summary>
public class M2MqttUnityUR : M2MqttUnityClient
{
    // Public fields
    public PolyscopeRobot polyscopeRobot;
    [Header("MQTT")]
    public string topic = "test/json";
    [Header("User Interface")]
    public Toggle encryptedToggle;
    public InputField addressInputField;
    public InputField portInputField;
    public Button connectButton;
    public Button disconnectButton;
    public Button publishButton;

    // Private fields
    private List<string> _eventMessages = new();
    private bool _updateUI = false;
    private List<PolyscopeRobot.JointTransformAndAxis> _jointTransformAndAxisList;
    private bool _firstMessage = true;
    
    // Nested class for JSON payload
    [Serializable]
    public class JsonPayload
    {
        public string processProgress;
        public float[] tcpPose;
        public float[] jointPositions;
        public bool toolStatus;
        public int counterData;
        public bool processBusy;
    }
    
    /// <summary>
    /// Set the MQTT broker address.
    /// </summary>
    /// <param name="brokerAddress">The address of the MQTT broker.</param>
    public void SetBrokerAddress(string brokerAddress)
    {
        if (addressInputField && !_updateUI)
        {
            this.brokerAddress = brokerAddress;
        }
    }

    /// <summary>
    /// Set the MQTT broker port.
    /// </summary>
    /// <param name="brokerPort">The port of the MQTT broker.</param>
    public void SetBrokerPort(string brokerPort)
    {
        if (portInputField && !_updateUI)
        {
            int.TryParse(brokerPort, out this.brokerPort);
        }
    }

    /// <summary>
    /// Set the MQTT connection encryption.
    /// </summary>
    /// <param name="isEncrypted">True if the connection should be encrypted, false otherwise.</param>
    public void SetEncrypted(bool isEncrypted)
    {
        this.isEncrypted = isEncrypted;
    }
    
    /// <summary>
    /// Called when the MQTT client is connecting.
    /// </summary>
    protected override void OnConnecting()
    {
        base.OnConnecting();
    }

    /// <summary>
    /// Subscribe to topics for the MQTT client.
    /// </summary>
    protected override void SubscribeTopics()
    {
        client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
    }

    /// <summary>
    /// Unsubscribe from topics for the MQTT client.
    /// </summary>
    protected override void UnsubscribeTopics()
    {
        client.Unsubscribe(new string[] { topic });
    }

    /// <summary>
    /// Called when the MQTT client connection fails.
    /// </summary>
    /// <param name="errorMessage">The error message associated with the connection failure.</param>
    protected override void OnConnectionFailed(string errorMessage)
    {
        Debug.LogError("CONNECTION FAILED! " + errorMessage);
    }

    /// <summary>
    /// Called when the MQTT client is disconnected.
    /// </summary>
    protected override void OnDisconnected()
    {
        base.OnDisconnected();
        UpdateUI();
    }

    protected override void OnConnected()
    {
        base.OnConnected();
        UpdateUI();
    }

    /// <summary>
    /// Called when the MQTT client connection is lost.
    /// </summary>
    protected override void OnConnectionLost()
    {
        Debug.LogWarning("CONNECTION LOST!");
    }

    /// <summary>
    /// Update the user interface elements based on the MQTT client connection state.
    /// </summary>
    private void UpdateUI()
    {
        if (client == null)
        {
            if (connectButton != null)
            {
                connectButton.interactable = true;
                disconnectButton.interactable = false;
            }
        }
        else
        {
            if (disconnectButton != null)
            {
                disconnectButton.interactable = client.IsConnected;
            }
            if (connectButton != null)
            {
                connectButton.interactable = !client.IsConnected;
            }
        }

        if (addressInputField != null)
        {
            addressInputField.interactable = connectButton != null && connectButton.interactable;
            addressInputField.text = brokerAddress;
        }

        if (portInputField != null)
        {
            portInputField.interactable = connectButton != null && connectButton.interactable;
            portInputField.text = brokerPort.ToString();
        }

        if (encryptedToggle != null)
        {
            encryptedToggle.interactable = connectButton != null && connectButton.interactable;
            encryptedToggle.isOn = isEncrypted;
        }

        if (publishButton != null)
        {
            publishButton.interactable = disconnectButton != null && disconnectButton.interactable;
        }

        _updateUI = false;
    }

    /// <summary>
    /// Called when the script is initialized.
    /// </summary>
    protected override void Start()
    {
        Debug.Log("Ready.");
        _updateUI = true;
        base.Start();
        
        _jointTransformAndAxisList = polyscopeRobot.GetJointTransformsAndEnabledRotationAxis();
    }

    /// <summary>
    /// Decode the received MQTT message.
    /// </summary>
    /// <param name="topic">The topic the message was received on.</param>
    /// <param name="message">The message payload in bytes.</param>
    protected override void DecodeMessage(string topic, byte[] message)
    {
        string msg = System.Text.Encoding.UTF8.GetString(message);
        // Debug.Log("Received: " + msg);
        StoreMessage(msg);
    }

    /// <summary>
    /// Store a received message in the event messages list.
    /// </summary>
    /// <param name="eventMsg">The received message to store.</param>
    private void StoreMessage(string eventMsg)
    {
        _eventMessages.Add(eventMsg);
    }

    /// <summary>
    /// Process a received MQTT message.
    /// </summary>
    /// <param name="msg">The message to process.</param>
    private void ProcessMessage(string msg)
    {
        var jsonPayload = JsonConvert.DeserializeObject<JsonPayload>(msg);
        
        ReceiveTransformJoints(jsonPayload.jointPositions);
    }

    /// <summary>
    /// Update the joint positions of the robot based on the received joint positions.
    /// </summary>
    /// <param name="jointPositions">The array of joint positions.</param>
    private void ReceiveTransformJoints(float[] jointPositions)
    {
        if (jointPositions.Length != _jointTransformAndAxisList.Count)
        {
            Debug.LogError("Joint positions array and joint transform array have different lengths!");
            return;
        }
        
        for (int i = 0; i < jointPositions.Length; i++)
        {
            float angle = jointPositions[i];
            if (angle >= -Mathf.PI && angle <= Mathf.PI)
            {
                angle *= Mathf.Rad2Deg;
            }

            switch (i)
            {
                case 1:
                case 3:
                    angle += 90;
                    break;
                case 0:
                case 4:
                    angle += 180;
                    break;
            }

            _jointTransformAndAxisList[i].JointTransform.localRotation = Quaternion.AngleAxis(angle, _jointTransformAndAxisList[i].EnabledRotationAxis);
        }
    }

    public void PublishTransformJoints()
    {
        var jointTransformAndAxisList = polyscopeRobot.GetJointTransformsAndEnabledRotationAxis();
        List<float>jointAngles = new List<float>(jointTransformAndAxisList.Count);

        for (int i = 0; i < jointTransformAndAxisList.Count; i++)
        {
            var angle = polyscopeRobot.GetJointRotationAngle(jointTransformAndAxisList[i]);
            angle = RobotsHelper.WrapAngle(angle);
            
            switch (i)
            {
                case 1:
                case 3:
                    angle -= 90;
                    break;
                case 0:
                case 4:
                    angle += 180;
                    break;
            }
            
            angle *= Mathf.Deg2Rad;
            jointAngles.Add(angle);
        }

        // Use object initializer to create JsonPayload
        var jsonPayload = new JsonPayload { jointPositions = jointAngles.ToArray() };

        // Use JsonUtility.ToJson overload to avoid creating data string
        var data = JsonUtility.ToJson(jsonPayload, false);

        // Use Encoding.UTF8.GetBytes overload to avoid creating separate byte array
        client.Publish("test/json", System.Text.Encoding.UTF8.GetBytes(data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        Debug.Log("Joint message published:" + data);
    }
    

    /// <summary>
    /// Called once per frame.
    /// </summary>
    protected override void Update()
    {
        base.Update(); // call ProcessMqttEvents()

        if (_eventMessages.Count > 0 && _firstMessage)
        {
            foreach (string msg in _eventMessages)
            {
                ProcessMessage(msg);
            }
            _eventMessages.Clear();
            _firstMessage = false;
        }
        if (_updateUI)
        {
            UpdateUI();
        }
        
    }

    /// <summary>
    /// Called when the script is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        Disconnect();
    }
}