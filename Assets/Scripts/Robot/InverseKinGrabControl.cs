using System;
using System.Collections;
using System.Collections.Generic;
using PimDeWitte.UnityMainThreadDispatcher;
using Schema.Socket.Internals;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class InverseKinGrabControl : MonoBehaviour
{
    public RobotManager robotManager;
    public SessionClient sessionClient;
    private List<double> _newPose = new (6) {0f,0f,0f,0f,0f,0f};
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grabInteractable;

    private void OnEnable()
    {
        _grabInteractable.selectEntered.AddListener(ValidateInverseKinematics);
    }

    void Start()
    {
        sessionClient.OnConnected += () => UnityMainThreadDispatcher.Instance().Enqueue(ValidateNewPosition);
    }

    private void ValidateInverseKinematics(SelectEnterEventArgs selectEnter)
    {
        ValidateNewPosition();
    }

    void OnIKSuccessAction()
    {
        Debug.Log("IK Success");
    }

    void ValidateNewPosition()
    {
        // Construct and send IK Data Request
        InternalsGetInverseKinIn ikDataRequest = new InternalsGetInverseKinIn
        {
            MaxPositionError = null,
            Qnear = null,
            TcpOffset = null,
            X = robotManager.qActualJoints
        };
        sessionClient.SendInverseKinematicsRequest(ikDataRequest, OnIKSuccessAction);
    }

    public void OnRelease()
    {
        ValidateNewPosition();
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
