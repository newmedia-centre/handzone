using System;
using System.Collections.Generic;
using UnityEngine;

public class RobotTranslator : MonoBehaviour
{
    public Transform[] robotPivots;
    public Vector3[] rotationDirection;
    public float smoothness = 20f;
    public bool matchPivots = false;
    public float[] jointAngles;
    
    private GameObject[] _currentTransforms;
    private List<Transform> _ghRobotPivots = new();
    private List<Transform> _alignedGhPivots = new();
    private bool _polyscopePivotChanged;
    private bool _ghPivotChanged;
    
    public static Action<int, float> UpdatePolyscopeJoint;
    public static Action<int, float> MovePolyscopeJoint;
    public static Action<int, float> OnJointChanged;
    
    private void Awake()
    {
        UpdatePolyscopeJoint += SetCurrentJoint;
        MovePolyscopeJoint += MoveCurrentJoint;
    }

    private void Start()
    {
        // Make sure to skip the first child, which is the robot base
        for(int i = 1; i < transform.childCount; ++i){
            Transform ghRobotPivot = transform.GetChild(i);
            _ghRobotPivots.Add(ghRobotPivot);
        }
        _currentTransforms = new GameObject[_ghRobotPivots.Count];
        jointAngles = new float[_currentTransforms.Length];
        
        for (int i = 0; i < _ghRobotPivots.Count; i++)
        {
            GameObject robotPivot = new GameObject($"{_ghRobotPivots[i].name} Pivot {i}");
            robotPivot.transform.parent = _ghRobotPivots[i];
            robotPivot.transform.SetPositionAndRotation(robotPivots[i].position, robotPivots[i].rotation);
            _alignedGhPivots.Add(robotPivot.transform);
            
            _currentTransforms[i] = new GameObject($"Robot Joints Poser {i}");
        }
    }

    private void Update()
    {
        if (matchPivots)
        {
            for (int i = 0; i < _currentTransforms.Length; i++)
            {
                if (robotPivots[i].hasChanged)
                {
                    _polyscopePivotChanged = true;
                    _ghPivotChanged = false;
                    _currentTransforms[i].transform.position = robotPivots[i].position;
                    _currentTransforms[i].transform.rotation = robotPivots[i].rotation;
                    robotPivots[i].hasChanged = false;
                    
                    jointAngles[i] = _currentTransforms[i].transform.localRotation.eulerAngles.magnitude;
                    OnJointChanged?.Invoke(i, jointAngles[i]);
                }

                if (_alignedGhPivots[i].hasChanged)
                {
                    _ghPivotChanged = true;
                    _polyscopePivotChanged = false;
                    _currentTransforms[i].transform.position = _alignedGhPivots[i].position;
                    _currentTransforms[i].transform.rotation = _alignedGhPivots[i].rotation;
                    _alignedGhPivots[i].hasChanged = false;
                    
                    jointAngles[i] = _currentTransforms[i].transform.localRotation.eulerAngles.magnitude;
                    OnJointChanged?.Invoke(i, jointAngles[i]);
                }
            }

            if (_polyscopePivotChanged && !_ghPivotChanged)
            {
                bool allCompleted = true;
                for (int i = 0; i < _ghRobotPivots.Count; i++)
                {
                    // Calculates the offsets from aligned gh pivot point to the current transform
                    Vector3 offsetPos = _alignedGhPivots[i].position - _currentTransforms[i].transform.position;
                    Quaternion offsetRot = _currentTransforms[i].transform.rotation * Quaternion.Inverse(_alignedGhPivots[i].rotation);
                    
                    // Applies the offsets to the gh pivot point
                    Quaternion targetRot = offsetRot * _ghRobotPivots[i].rotation;
                    Vector3 targetPos = _ghRobotPivots[i].position - offsetPos;
                    
                    _ghRobotPivots[i].position = targetPos;
                    _ghRobotPivots[i].rotation = Quaternion.Slerp(_ghRobotPivots[i].rotation, targetRot, Time.deltaTime * smoothness);
                    _alignedGhPivots[i].hasChanged = false;
                    
                    if (_alignedGhPivots[i].transform.position != _currentTransforms[i].transform.position)
                    {
                        allCompleted = false;
                    }
                }
                
                if (allCompleted)
                {
                    _polyscopePivotChanged = false;
                }
            }
            else if (_ghPivotChanged && !_polyscopePivotChanged)
            {
                bool allCompleted = true;

                for (int i = 0; i < robotPivots.Length; i++)
                {
                    robotPivots[i].position = _currentTransforms[i].transform.position;
                    robotPivots[i].rotation = _currentTransforms[i].transform.rotation;
                    robotPivots[i].hasChanged = false;
                    
                    if(robotPivots[i].transform.position != _currentTransforms[i].transform.position)
                    {
                        allCompleted = false;
                    }
                }

                if (allCompleted)
                {
                    _ghPivotChanged = false;
                }
            }
        }
    }

    void SendJointsOverIP()
    {
        
        UR_EthernetIPClient.UpdateMovej(ToPolyscopeAngles(jointAngles));
    }

    public float[] ToPolyscopeAngles(float[] joints)
    {
        // TODO: Fix correct angles to match rotation with polyscope simulator
        
        float[] angles = new float[joints.Length];
        for (int i = 0; i < joints.Length; i++)
        {
            switch (i)
            {
                case 0:
                case 4:
                    angles[i] = -joints[i];
                    break;
                case 1:
                case 2:
                case 3:
                    angles[i] -= 90;
                    break;
                    
            }
            angles[i] = RobotsHelper.WrapAngle(angles[i]);
            angles[i] *= Mathf.Deg2Rad;
        }

        return angles;
    }
    
    public float GetCurrentJointAngle(int index)
    {
        return jointAngles[index];
    }
    
    void SetCurrentJoint(int index, float angle)
    {
        robotPivots[index].transform.localRotation = Quaternion.Euler(rotationDirection[index] * angle);
        SendJointsOverIP();
    }
    
    void MoveCurrentJoint(int index, float direction)
    {
        robotPivots[index].transform.localRotation *= Quaternion.Euler(rotationDirection[index] * direction);
        SendJointsOverIP();
    }
    
    public void UpdatePolyscopeJoints(float[] joints)
    {
        for (int i = 0; i < joints.Length; i++)
        {
            float angle = joints[i] * Mathf.Rad2Deg;
            switch (i)
            {
                case 0:
                case 4:
                    angle = -angle;
                    break;
                case 1:
                case 3:
                    angle += 90;
                    break;
            }
            robotPivots[i].transform.localRotation = Quaternion.Euler(rotationDirection[i] * angle);
        }
    }
}
