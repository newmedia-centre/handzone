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