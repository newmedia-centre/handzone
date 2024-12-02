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

#endregion

/// <summary>
/// The Gripper class manages the functionality of a robotic gripper in a Unity environment. It handles the grabbing and releasing of objects, maintaining the state of the currently grabbed object, and managing the attachment point for the gripper. The class listens for tool loading and unloading events to trigger grabbing and releasing actions. It also updates the position and rotation of the grabbed object to follow the gripper's anchor.
/// </summary>
public class Gripper : MonoBehaviour
{
    public GameObject grabbableObject;
    public GameObject currentGrabbedObject;

    private bool _isGrabbing;
    private GameObject _attachAnchor;
    private bool _previousKinematicSetting;

    /// <summary>
    /// Initializes the Gripper by creating an attachment anchor and setting up event listeners
    /// for tool loading and unloading.
    /// </summary>
    private void Awake()
    {
        _attachAnchor = new GameObject(name + "Attach Anchor");
        _attachAnchor.transform.SetParent(transform);
    }

    /// <summary>
    /// Subscribes to events for tool loading and unloading when the gripper is enabled.
    /// </summary>
    private void OnEnable()
    {
        RobotActions.OnToolLoaded += Grab;
        RobotActions.OnToolUnloaded += UnGrab;
    }

    /// <summary>
    /// Unsubscribes from events for tool loading and unloading when the gripper is disabled.
    /// </summary>
    private void OnDisable()
    {
        RobotActions.OnToolLoaded -= Grab;
        RobotActions.OnToolUnloaded -= UnGrab;
    }

    /// <summary>
    /// Detects when a grabbable object enters the trigger area of the gripper.
    /// </summary>
    /// <param name="other">The collider of the object that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        grabbableObject = other.gameObject;
    }

    /// <summary>
    /// Detects when a grabbable object exits the trigger area of the gripper.
    /// </summary>
    /// <param name="other">The collider of the object that exited the trigger.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == grabbableObject) grabbableObject = null;
    }

    /// <summary>
    /// Releases the currently grabbed object, if any, and updates the gripper's state.
    /// </summary>
    public void UnGrab()
    {
        _isGrabbing = false;

        if (currentGrabbedObject)
        {
            RobotActions.OnToolUngrabbed(currentGrabbedObject);
            currentGrabbedObject = null;
        }
    }

    /// <summary>
    /// Grabs the grabbable object, if available, and updates the gripper's state.
    /// </summary>
    public void Grab()
    {
        if (grabbableObject)
        {
            currentGrabbedObject = grabbableObject;
            RobotActions.OnToolGrabbed(currentGrabbedObject);

            grabbableObject = null;
            _isGrabbing = true;
        }
    }

    /// <summary>
    /// Updates the position and rotation of the currently grabbed object to follow the
    /// attachment anchor of the gripper.
    /// </summary>
    private void Update()
    {
        if (_isGrabbing)
            currentGrabbedObject.transform.SetPositionAndRotation(_attachAnchor.transform.position,
                _attachAnchor.transform.rotation);
    }

    /// <summary>
    /// Sets the position of the attachment anchor for the gripper.
    /// </summary>
    /// <param name="position">The new local position for the attachment anchor.</param>
    public void SetAnchorPosition(Vector3 position)
    {
        _attachAnchor.transform.localPosition = position;
    }
}