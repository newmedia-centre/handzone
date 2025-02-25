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
using Schema.Socket.Realtime;
using UnityEngine;

#endregion

/// <summary>
/// The GripperAnim class manages the animation of a robotic gripper in Unity.
/// It handles the initialization of the animator and gripper components,
/// and responds to digital output changes from a session client to control
/// the gripping state of the robotic arm.
/// </summary>
[RequireComponent(typeof(Animator))]
public class GripperAnim : MonoBehaviour
{
    private Animator _animController;
    private Gripper _gripper;

    /// <summary>
    /// Initializes the animator and gripper components, and sets up event listeners
    /// for digital output changes from the session client.
    /// </summary>
    private void Start()
    {
        _animController = GetComponent<Animator>();

        _gripper = GetComponent<Gripper>();
        _gripper.SetAnchorPosition(new Vector3(0, 0, -1.453f));

        if (SessionClient.Instance == null)
        {
            return;
        }

        SessionClient.Instance.OnDigitalOutputChanged += SetGripperAnim;
        SessionClient.Instance.OnRealtimeData += SetGripperAnim;
    }

    /// <summary>
    /// Sets the gripper animation state based on the received RealtimeDataOut object.
    /// </summary>
    /// <param name="obj">The RealtimeDataOut object containing digital output data.</param>
    private void SetGripperAnim(RealtimeDataOut obj)
    {
        SetGripperAnim(Convert.ToBoolean(obj.DigitalOutputs));
    }

    /// <summary>
    /// Updates the gripper animation state and performs gripping or ungripping actions
    /// based on the provided state.
    /// </summary>
    /// <param name="state">True to grip, false to ungrip.</param>
    private void SetGripperAnim(bool state)
    {
        _animController.SetBool("Gripping", !state);

        if (state == true)
            _gripper.Grab();
        else
            _gripper.UnGrab();
    }
}