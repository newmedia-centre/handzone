// Copyright 2024 NewMedia Centre - Delft University of Technology
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#region

using System;
using System.Collections.Generic;
using Schema.Socket.Internals;
using Schema.Socket.Realtime;
using Unity.VisualScripting;
using UnityEngine;

#endregion

/// <summary>
/// The RobotManager class is responsible for managing the state and behavior
/// of a robotic system within a Unity environment. It handles the initialization
/// of robot joints, manages the current state of the robot's joints, and
/// facilitates communication with external systems for controlling the robot.
/// The class also listens for real-time data updates and processes inverse
/// kinematics requests to ensure accurate movement and positioning of the robot.
/// </summary>
public class RobotManager : MonoBehaviour
{
    [Header("Robot Joints")] public Transform[] robotJoints;
    public Vector3[] rotationDirection;
    public List<double> qActualJoints;
    private List<Vector3> _initialRotations = new();

    [Header("Prefab")] public GameObject transformGizmoPrefab;

    private List<Outline> _outlines = new();
    private List<BoxCollider> _colliders = new();
    private List<GameObject> _transformGizmos = new();

    /// <summary>
    /// Initializes the robot's joints and sets up event listeners for real-time data updates.
    /// This method is called when the script instance is being loaded.
    /// It initializes the joint rotations, colliders, and gizmos, and subscribes
    /// to the session client events for real-time data updates.
    /// </summary>
    private void Start()
    {
        foreach (var robotJoint in robotJoints)
        {
            _initialRotations.Add(robotJoint.localRotation.eulerAngles);

            _colliders.Add(robotJoint.AddComponent<BoxCollider>());
            _transformGizmos.Add(Instantiate(transformGizmoPrefab, robotJoint));
            _transformGizmos[^1].SetActive(false);
        }

        if (SessionClient.Instance == null)
        {
            Debug.LogWarning(
                "SessionClient instance is null. Make sure to have a SessionClient instance in the scene.");
            return;
        }

        SessionClient.Instance.OnRealtimeData += UpdateJointsFromPolyscope;
        SessionClient.Instance.OnKinematicCallback += UpdateJointsFromGrabbing;
    }

    /// <summary>
    /// Sets the current joint's angle based on the specified index and angle.
    /// </summary>
    /// <param name="index">The index of the joint to set.</param>
    /// <param name="angle">The angle to set the joint to, in degrees.</param>
    private void SetCurrentJoint(int index, float angle)
    {
        var newJoints = new float[robotJoints.Length];

        robotJoints[index].transform.localRotation = Quaternion.Euler(rotationDirection[index] * angle);

        for (var i = 0; i < robotJoints.Length; i++)
        {
            newJoints[i] = robotJoints[i].transform.localRotation.eulerAngles.magnitude;
            newJoints[i] = FixAngle(newJoints[i], index);
        }
    }

    /// <summary>
    /// Updates the robot's joints based on the provided inverse kinematics data.
    /// </summary>
    /// <param name="data">A list of joint angles to update, represented as doubles.</param>
    public void UpdateJoints(List<double> data)
    {
        if (data == null) return;

        for (var i = 0; i < data.Count; i++)
        {
            qActualJoints[i] = data[i];
            var angle = (float)(qActualJoints[i] * Mathf.Rad2Deg);
            // if (i == 0 || i == 4) angle = -angle;
            if (i == 1 || i == 3) angle += 90;

            robotJoints[i].localRotation = Quaternion.Euler(_initialRotations[i] + rotationDirection[i] * angle);
        }
    }

    public void UpdateFromInverseKinematics(List<double> target, Action function)
    {
        var data = new InternalsGetInverseKinIn();
        data.Qnear = qActualJoints;
        data.MaxPositionError = 0.001;
        data.X = target;

        SessionClient.Instance.SendInverseKinematicsRequest(data, function);
    }

    /// <summary>
    /// Updates the robot's joints based on the provided inverse kinematics data
    /// from the Polyscope system.
    /// </summary>
    /// <param name="data">The real-time data containing joint angles.</param>
    public void UpdateJointsFromPolyscope(RealtimeDataOut data)
    {
        if (data == null) return;

        UpdateJoints(data.QActual);
    }

    /// <summary>
    /// Updates the robot's joints based on the provided inverse kinematics data
    /// from a grabbing action.
    /// </summary>
    /// <param name="data">The inverse kinematics callback data containing joint angles.</param>
    public void UpdateJointsFromGrabbing(InternalsGetInverseKinCallback data)
    {
        if (data == null) return;

        UpdateJoints(data.Ik);
    }
    
    private void UpdateJoints(List<double> data)
    {
        if (data == null) return;

        for (var i = 0; i < data.Count; i++)
        {
            qActualJoints[i] = data[i];
            var angle = (float)(qActualJoints[i] * Mathf.Rad2Deg);
            // if (i == 0 || i == 4) angle = -angle;
            if (i == 1 || i == 3) angle += 90;

            robotJoints[i].localRotation = Quaternion.Euler(_initialRotations[i] + rotationDirection[i] * angle);
        }
    }
    
    /// <summary>
    /// Converts joint angles from the robot's local coordinate system to the
    /// Polyscope coordinate system.
    /// </summary>
    /// <param name="joints">An array of joint angles in degrees.</param>
    /// <returns>An array of joint angles converted to the Polyscope coordinate system.</returns>
    public float[] ToPolyscopeAngles(float[] joints)
    {
        for (var i = 0; i < joints.Length; i++)
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

    /// <summary>
    /// Fixes the angle of a joint based on its index to ensure it is within
    /// the correct range for the robot's movement.
    /// </summary>
    /// <param name="joint">The angle of the joint to fix.</param>
    /// <param name="index">The index of the joint being fixed.</param>
    /// <returns>The fixed angle of the joint.</returns>
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