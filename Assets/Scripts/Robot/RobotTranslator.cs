using Schema.Socket.Internals;
using Schema.Socket.Realtime;
using System;
using UnityEngine;

public class RobotTranslator : MonoBehaviour
{
    public Transform[] robotPivots;
    public Vector3[] rotationDirection;
    public double[] qActualJoints;
    
    private GameObject[] _currentTransforms;
    
    private void Awake()
    {
        // Keep track of the current joint angles
        qActualJoints = new double[robotPivots.Length];
        
        WebClient.OnRealtimeData += UpdateJointsFromPolyscope;
        WebClient.OnKinematicCallback += UpdateJointsFromGrabbing;
    }
    
    void SetCurrentJoint(int index, float angle)
    {
        float[] newJoints = new float[robotPivots.Length];

        robotPivots[index].transform.localRotation = Quaternion.Euler(rotationDirection[index] * angle);
        
        for (int i = 0; i < robotPivots.Length; i++)
        {
            newJoints[i] = robotPivots[i].transform.localRotation.eulerAngles.magnitude;
            newJoints[i] = FixAngle(newJoints[i], index);
        }
    }

    public void UpdateFromInverseKinematics(double[] target, Action function)
    {
        InternalsGetInverseKinIn data = new InternalsGetInverseKinIn();
        data.Qnear = qActualJoints;
        data.MaxPositionError = 0.001;
        data.X = target;

        WebClient.Instance.SendInverseKinematicsRequest(data, function);
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

    private void UpdateJoints(double[] data)
    {
        if (data == null) return;

        for (int i = 0; i < data.Length; i++)
        {
            qActualJoints[i] = data[i];
            float angle = (float)(qActualJoints[i] * Mathf.Rad2Deg);
            if (i == 0 || i == 4) angle = -angle;
            else if (i == 1 || i == 3) angle += 90;
            
            robotPivots[i].localRotation = Quaternion.Euler(rotationDirection[i] * angle);
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
