using Schema.Socket.Internals;
using Schema.Socket.Realtime;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    [Header("Robot Joints")]
    public Transform[] robotJoints;
    public Vector3[] rotationDirection;
    public List<double> qActualJoints;
    private List<Vector3> _initialRotations = new();
    
    [Header("Prefab")]
    public GameObject transformGizmoPrefab;

    [Header("Outlines")]
    public bool enableOutlines = true;
    public Color outlineColor = Color.yellow;
    public float outlineWidth = 3f;
    
    private List<Outline> _outlines = new();
    private List<BoxCollider> _colliders = new();
    private List<GameObject> _transformGizmos = new();

    private void Awake()
    {

    }

    private void Start()
    {
        foreach (var robotJoint in robotJoints)
        {
            _initialRotations.Add(robotJoint.localRotation.eulerAngles);
            
            _colliders.Add(robotJoint.AddComponent<BoxCollider>());
            _transformGizmos.Add(Instantiate(transformGizmoPrefab, robotJoint));
            var outline = robotJoint.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = outlineWidth;
            outline.enabled = enableOutlines;
            _outlines.Add(outline);
        }
        
        if(SessionClient.Instance == null)
        {
            Debug.LogWarning("SessionClient instance is null. Make sure to have a SessionClient instance in the scene.");
            return;
        }
        SessionClient.Instance.OnRealtimeData += UpdateJointsFromPolyscope;
        SessionClient.Instance.OnKinematicCallback += UpdateJointsFromGrabbing;
    }
    
    void SetCurrentJoint(int index, float angle)
    {
        float[] newJoints = new float[robotJoints.Length];

        robotJoints[index].transform.localRotation = Quaternion.Euler(rotationDirection[index] * angle);
        
        for (int i = 0; i < robotJoints.Length; i++)
        {
            newJoints[i] = robotJoints[i].transform.localRotation.eulerAngles.magnitude;
            newJoints[i] = FixAngle(newJoints[i], index);
        }
    }

    public void UpdateFromInverseKinematics(List<double> target, Action function)
    {
        InternalsGetInverseKinIn data = new InternalsGetInverseKinIn();
        data.Qnear = qActualJoints;
        data.MaxPositionError = 0.001;
        data.X = target;

        SessionClient.Instance.SendInverseKinematicsRequest(data, function);
    }

    public void UpdateJointsFromPolyscope(RealtimeDataOut data)
    {
        if (data == null) return;
        
        UpdateJoints(data.QActual);
    }

    public void UpdateJointsFromGrabbing(InternalsGetInverseKinCallback data)
    {
        if (data == null) return;

        UpdateJoints(data.Ik);
    }

    private void UpdateJoints(List<double> data)
    {
        if (data == null) return;

        for (int i = 0; i < data.Count; i++)
        {
            qActualJoints[i] = data[i];
            float angle = (float)(qActualJoints[i] * Mathf.Rad2Deg);
            // if (i == 0 || i == 4) angle = -angle;
            if (i == 1 || i == 3) angle += 90;
            
            robotJoints[i].localRotation = Quaternion.Euler(_initialRotations[i] + rotationDirection[i] * angle);
        }
    }
    
    public float[] ToPolyscopeAngles(float[] joints)
    {
        for (int i = 0; i < joints.Length; i++)
        {
            switch (i)
            {
                case 0:
                case 4:
                    joints[i] -= joints[i];
                    break;
                case 1:
                case 3:
                    joints[i] -= 90;
                    break;
            }
            joints[i] = RobotsHelper.WrapAngle(joints[i]);
            joints[i] *= Mathf.Deg2Rad;
        }
        return joints;
    }

    public float FixAngle(float joint, int index)
    {
        switch (index)
        {
            case 0:
            case 4:
                joint += joint;
                break;
            case 1:
            case 3:
                joint += 90;
                break;
        }
        return joint;
    }
}
