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
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

#endregion

public class RobotJointInteractable : XRBaseInteractable
{
    /// <summary>
    /// Configure the joint rotation axis on which axis the joint rotates with.
    /// </summary>
    [Header("Joint Interactable Configuration")]
    public float grabDistanceThreshold = 0.03f;

    public bool isControllerBeyondThreshold = false;

    private Vector3 _originGrabPosition;
    private Quaternion _originJointRotation;

    [Header("Debug")] public bool _enableDebug = false;
    public GameObject originInteractorPreviewPrefab;

    private GameObject _originInteractorPreview;

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);

        // Only allow one interactor to grab this interactable at a time
        if (interactorsSelecting.Count == 1)
        {
            // Store the origin interactor position
            _originGrabPosition = args.interactorObject.transform.position;

            // Store the origin rotation
            _originJointRotation = transform.localRotation;

            Grab();

            if (_enableDebug) CreateOriginInteractorPreview();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        // Drop the object if there are no interactors selecting it
        if (interactorsSelecting.Count == 0)
        {
            Drop();

            if (_enableDebug) DeleteOriginInteractorPreview();
        }
    }

    private void Update()
    {
        if (isSelected)
        {
            // Check if the controller position is beyond a threshold distance from the origin position
            if (!isControllerBeyondThreshold &&
                Vector3.Distance(_originGrabPosition, this.GetOldestInteractorSelecting().transform.position) <
                grabDistanceThreshold) return;

            // Set _isBeyondThreshold to true if it was false
            if (!isControllerBeyondThreshold) isControllerBeyondThreshold = true;

            // Update the rotation based on the position of the oldest interactor selecting the object
            UpdateRotation();
        }
    }

    /// <summary>
    /// Updates the rotation of the object based on the position of the oldest interactor selecting it.
    /// </summary>
    private void UpdateRotation()
    {
        var upDirection = transform.up;
        var currentPosPlane =
            CalculatePositionPlane(this.GetOldestInteractorSelecting().transform.position - _originGrabPosition,
                upDirection);
        var originPosPlane = CalculatePositionPlane(Vector3.one, upDirection);

        var targetRotation = CalculateTargetRotation(originPosPlane, currentPosPlane);
        transform.localRotation = _originJointRotation * targetRotation;
    }

    /// <summary>
    /// Calculates the position of a point on a plane defined by the up direction.
    /// </summary>
    /// <param name="position">The position of the point.</param>
    /// <param name="upDirection">The up direction of the plane.</param>
    /// <returns>The position of the point projected onto the plane, normalized.</returns>
    private Vector3 CalculatePositionPlane(Vector3 position, Vector3 upDirection)
    {
        var positionPlane = Vector3.ProjectOnPlane(position, upDirection);
        return positionPlane.normalized;
    }

    /// <summary>
    /// Calculates the target rotation based on the origin and current position planes.
    /// </summary>
    /// <param name="originPosPlane">The position plane of the origin.</param>
    /// <param name="currentPosPlane">The position plane of the current position.</param>
    /// <returns>The target rotation quaternion.</returns>
    private Quaternion CalculateTargetRotation(Vector3 originPosPlane, Vector3 currentPosPlane)
    {
        var cross = Vector3.Cross(originPosPlane, currentPosPlane);
        var dot = Vector3.Dot(originPosPlane, currentPosPlane) + 1.0f;
        var targetRotation = new Quaternion(cross.x, cross.y, cross.z, dot);
        return targetRotation.normalized;
    }

    /// <summary>
    /// Updates the state of the object due to being grabbed.
    /// Automatically called when entering the Select state.
    /// </summary>
    protected virtual void Grab()
    {
    }

    protected virtual void Drop()
    {
        isControllerBeyondThreshold = false;
    }

    private void CreateOriginInteractorPreview()
    {
        if (originInteractorPreviewPrefab == null) return;

        if (_originInteractorPreview != null) DeleteOriginInteractorPreview();
        _originInteractorPreview = Instantiate(originInteractorPreviewPrefab, _originGrabPosition, Quaternion.identity);
    }

    private void DeleteOriginInteractorPreview()
    {
        Destroy(_originInteractorPreview);
    }
}