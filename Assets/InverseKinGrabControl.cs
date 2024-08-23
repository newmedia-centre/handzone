using System;
using System.Collections;
using System.Collections.Generic;
using PimDeWitte.UnityMainThreadDispatcher;
using Schema.Socket.Internals;
using UnityEngine;

public class InverseKinGrabControl : MonoBehaviour
{
    public GameObject referencedJoint;
    public SessionClient sessionClient;
    private  List<double> _newPose = new (6) {0f,0f,0f,0f,0f,0f};

    void Start()
    {
        sessionClient.OnConnected += () => UnityMainThreadDispatcher.Instance().Enqueue(ValidateNewPosition);
    }

    void OnIKSuccessAction()
    {
        //Update IK pose with _newPose
        sessionClient.SetTCP(_newPose.ToArray());
        print("IK Pose Updated");
    }

    void ValidateNewPosition()
    {
        
        print("Validating New IK Pose");
        // Update _newPose from referencedJoint's local transform
        if (referencedJoint)
        {
            // _newPose.Clear();
            print(referencedJoint.ToString());
            print(referencedJoint.transform.localPosition.ToString());
            Vector3 position = referencedJoint.transform.localPosition + transform.localPosition;
            Vector3 rotation = (transform.localRotation * referencedJoint.transform.localRotation).eulerAngles * Mathf.Deg2Rad;
            Debug.Log("Position is:" + position.ToString());
            Debug.Log("Rotation is:" + rotation.ToString());
            _newPose[0] = position.z / 10;
            _newPose[1] = position.y / 10;
            _newPose[2] = position.x / 10;
            _newPose[3] = rotation.x;
            _newPose[4] = rotation.y;
            _newPose[5] = rotation.z;
        }
        
        // Construct and send IK Data Request
        InternalsGetInverseKinIn ikDataRequest = new InternalsGetInverseKinIn
        {
            MaxPositionError = null,
            Qnear = null,
            TcpOffset = null,
            X = _newPose
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
