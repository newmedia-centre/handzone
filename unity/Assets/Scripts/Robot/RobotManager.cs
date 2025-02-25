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