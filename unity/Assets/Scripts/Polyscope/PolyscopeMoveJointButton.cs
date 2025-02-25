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

using UnityEngine;
using UnityEngine.EventSystems;

#endregion

/// <summary>
/// The PolyscopeMoveJointButton class handles the interaction of a button that
/// controls the movement of a robotic joint in a Polyscope environment. It implements
/// the IPointerDownHandler and IPointerUpHandler interfaces to manage the button's
/// pressed state, enabling or disabling inverse kinematics (IK) as needed. The class
/// also manages the direction and index of the joint being controlled.
/// </summary>
public class PolyscopeMoveJointButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int jointIndex;
    public int direction;
    private bool _isHeld;

    /// <summary>
    /// Called when the button is pressed down. It sets the held state to true
    /// and disables inverse kinematics for the robot.
    /// </summary>
    /// <param name="eventData">The pointer event data associated with the button press.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        _isHeld = true;
        PolyscopeRobot.DisableIK?.Invoke();
    }

    /// <summary>
    /// Called when the button is released. It sets the held state to false,
    /// enables inverse kinematics for the robot, and clears the send buffer.
    /// </summary>
    /// <param name="eventData">The pointer event data associated with the button release.</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        _isHeld = false;
        PolyscopeRobot.EnableIK?.Invoke();
        UR_EthernetIPClient.ClearSendBuffer();
    }

    /// <summary>
    /// Updates the state of the joint movement if the button is held down.
    /// This method is called once per frame.
    /// </summary>
    private void Update()
    {
        if (_isHeld)
        {
            // RobotTranslator.MovePolyscopeJoint?.Invoke(jointIndex, direction);
            // TODO: CHange to use WebClient event speedj
        }
    }
}