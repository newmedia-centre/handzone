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

using System.Collections.Generic;
using PimDeWitte.UnityMainThreadDispatcher;
using Schema.Socket.Internals;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

#endregion

/// <summary>
/// The InverseKinGrabControl class manages the inverse kinematics (IK) control
/// for a robotic system. It handles the selection and validation of inverse
/// kinematics for the robot's movements, ensuring that the robot can reach
/// specified positions and orientations accurately. The class listens for
/// events related to the robot's interaction and updates the robot's pose
/// based on the current state of the session client. It also manages the
/// grabbing and releasing of objects in the environment.
/// </summary>
public class InverseKinGrabControl : MonoBehaviour
{
    public RobotManager robotManager;
    public SessionClient sessionClient;
    private List<double> _newPose = new(6) { 0f, 0f, 0f, 0f, 0f, 0f };
    [SerializeField] private XRGrabInteractable _grabInteractable;

    /// <summary>
    /// Subscribes to the necessary events when the component is enabled.
    /// </summary>
    private void OnEnable()
    {
        _grabInteractable.selectEntered.AddListener(ValidateInverseKinematics);
    }

    /// <summary>
    /// Initializes the component and subscribes to session client events
    /// when the component starts.
    /// </summary>
    private void Start()
    {
        sessionClient.OnConnected += () => UnityMainThreadDispatcher.Instance().Enqueue(ValidateNewPosition);
    }

    /// <summary>
    /// Validates the inverse kinematics when an object is selected.
    /// </summary>
    /// <param name="selectEnter">The event data for the selection event.</param>
    private void ValidateInverseKinematics(SelectEnterEventArgs selectEnter)
    {
        ValidateNewPosition();
    }

    /// <summary>
    /// Called when the inverse kinematics operation is successful.
    /// </summary>
    private void OnIKSuccessAction()
    {
        Debug.Log("IK Success");
    }

    /// <summary>
    /// Validates and sends the new position for the robot's end effector
    /// based on the current state of the robot.
    /// </summary>
    private void ValidateNewPosition()
    {
        // Construct and send IK Data Request
        var ikDataRequest = new InternalsGetInverseKinIn
        {
            MaxPositionError = null,
            Qnear = null,
            TcpOffset = null,
            X = robotManager.qActualJoints
        };
        sessionClient.SendInverseKinematicsRequest(ikDataRequest, OnIKSuccessAction);
    }

    /// <summary>
    /// Releases the currently held object and resets the robot's position
    /// and rotation.
    /// </summary>
    public void OnRelease()
    {
        ValidateNewPosition();
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}