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

using Schema.Socket.Realtime;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

#endregion

/// <summary>
/// TCPController is a MonoBehaviour that controls the translation and rotation of an XRGrabInteractable object.
/// It listens to selectEntered and selectExited events of the XRGrabInteractable object and updates its pose based on RealtimeDataOut received from a WebClient.
/// </summary>
public class TCPController : MonoBehaviour
{
    private XRGrabInteractable _interactable;
    public bool _isGrabbed;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// It initializes the XRGrabInteractable component and adds listeners to its selectEntered and selectExited events.
    /// It also subscribes to the OnRealtimeData event of the WebClient, which is used to update the pose of the XRGrabInteractable object.
    /// </summary>
    private void Awake()
    {
        _interactable = GetComponent<XRGrabInteractable>();
        if (_interactable)
        {
            _interactable.selectEntered.AddListener(OnSelectEnter);
            _interactable.selectExited.AddListener(OnSelectExit);
        }
        else
        {
            Debug.Log("No XRGrabInteractable found, TCP interactable events will not work");
        }

        // WebClient.OnRealtimeData += UpdateRobotPose;
    }

    /// <summary>
    /// Called when the XRGrabInteractable object is selected.
    /// It updates the _isGrabbed flag based on the isSelected property of the interactableObject.
    /// </summary>
    /// <param name="selectEnterEventArgs">The arguments for the selectEntered event.</param>
    private void OnSelectEnter(SelectEnterEventArgs selectEnterEventArgs)
    {
        _isGrabbed = selectEnterEventArgs.interactableObject.isSelected;
    }

    /// <summary>
    /// Called when the XRGrabInteractable object is deselected.
    /// It updates the _isGrabbed flag based on the isSelected property of the interactableObject.
    /// </summary>
    /// <param name="selectExitEventArgs">The arguments for the selectExited event.</param>
    private void OnSelectExit(SelectExitEventArgs selectExitEventArgs)
    {
        _isGrabbed = selectExitEventArgs.interactableObject.isSelected;
    }

    /// <summary>
    /// Updates the pose of the XRGrabInteractable object based on the RealtimeDataOut received.
    /// If the object is not grabbed, it does nothing.
    /// </summary>
    /// <param name="data">The RealtimeDataOut received from the WebClient.</param>
    private void UpdateRobotPose(RealtimeDataOut data)
    {
        if (!_isGrabbed) return;

        var pose = new double[6];
        data.ToolVectorActual.CopyTo(pose, 0);

        // pose = AddCurrentTransformToPose(pose);

        SessionClient.Instance.MoveL(pose, 0.2, 0.25, 0, 0);
        _isGrabbed = false;
    }

    /// <summary>
    /// Updates the pose array with the current position and rotation of the XRGrabInteractable object.
    /// </summary>
    /// <param name="pose">The pose array to be updated.</param>
    private double[] AddCurrentTransformToPose(double[] pose)
    {
        var position = transform.localPosition;
        var rotation = transform.localRotation.eulerAngles * Mathf.Deg2Rad;
        Debug.Log(position);
        Debug.Log(rotation);

        // Update the position and rotation values in the pose array
        pose[0] += position.z / 10;
        pose[1] += position.y / 10;
        pose[2] += -position.x / 10;
        pose[3] += rotation.x;
        pose[4] += rotation.y;
        pose[5] += rotation.z;
        return pose;
    }
}