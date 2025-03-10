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
using UnityEngine;

#endregion

public class PolyscopeRobot : MonoBehaviour
{
    public static Action<JointTransformAndAxis, int> OnPolyscopeRotateJointToDirection;
    public static Action<JointTransformAndAxis, float> OnPolyscopeRotateJointToAngle;
    public static Action DisableIK;
    public static Action EnableIK;

    public float movementSpeed = 3.0f;

    // private List<BioJoint> _joints;
    // private BioSegment _tcpSegment;
    // private Transform _tcpTarget;
    // private BioIK.BioIK _bioIK;
    // private BioObjective[] _tcpObjectives;

    [Serializable]
    // Custom struct to store joint Transform and enabled rotation axis
    public struct JointTransformAndAxis
    {
        public Transform JointTransform;
        public Vector3 EnabledRotationAxis;

        public JointTransformAndAxis(Transform jointTransform, Vector3 enabledRotationAxis)
        {
            JointTransform = jointTransform;
            EnabledRotationAxis = enabledRotationAxis;
        }
    }

    //TODO: Add method to recieve excisting list of joint transforms and enabled rotation axis, instead of creating a new list every time.

    public List<JointTransformAndAxis> GetJointTransformsAndEnabledRotationAxis()
    {
        var jointTransformsAndAxes = new List<JointTransformAndAxis>();

        // foreach (var joint in _joints)
        // {
        //     Vector3 enabledRotationAxis = Vector3.zero;
        //
        //     if (joint.X.Enabled)
        //     {
        //         enabledRotationAxis = joint.X.Axis;
        //     }
        //     else if (joint.Y.Enabled)
        //     {
        //         enabledRotationAxis = joint.Y.Axis;
        //     }
        //     else if (joint.Z.Enabled)
        //     {
        //         enabledRotationAxis = joint.Z.Axis;
        //     }
        //
        //     jointTransformsAndAxes.Add(new JointTransformAndAxis(joint.transform, enabledRotationAxis));
        // }

        return jointTransformsAndAxes;
    }

    public float GetJointRotationAngle(JointTransformAndAxis joint)
    {
        var enabledRotationAxis = joint.EnabledRotationAxis;
        var currentRotation = joint.JointTransform.localRotation;
        var angle = 0f;

        if (enabledRotationAxis == Vector3.right)
            angle = currentRotation.eulerAngles.x;
        else if (enabledRotationAxis == Vector3.up)
            angle = currentRotation.eulerAngles.y;
        else if (enabledRotationAxis == Vector3.forward) angle = currentRotation.eulerAngles.z;

        return angle;
    }

    private void Awake()
    {
        // IK System for Polyscope Robot
        // _bioIK = GetComponent<BioIK.BioIK>();
        //
        //
        // // Get all the movable joints
        // _joints = new List<BioJoint>();
        // foreach (var bioSegment in _bioIK.Segments)
        // {
        //     if (bioSegment.Joint)
        //     {
        //         _joints.Add(bioSegment.Joint);
        //     }
        // }
        //
        // // Find the TCP objective on the robot
        // _tcpSegment = _bioIK.FindSegment("TCP");
        // if (_tcpSegment != null)
        // {
        //     _tcpObjectives = _tcpSegment.Objectives;
        //     BioObjective tcpObjective = _tcpSegment.Objectives[0];
        //
        //     if (tcpObjective is Position position)
        //     {
        //         _tcpTarget = position.GetTargetTransform();
        //     }
        // }

        // Subscribe to event listeners to perform translations on the robot IK system
        // if (_joints.Count > 0)
        // {
        //     OnPolyscopeRotateJointToDirection += RotateJointToDirection;
        //     OnPolyscopeRotateJointToAngle += RotateJointToAngle;
        //     DisableIK += DisableIKObjectives;
        //     EnableIK += EnableIKObjectives;
        // }
    }

    private void DisableIKObjectives()
    {
        // if(_tcpSegment == null)
        //     return;
        //
        // // Disable IK by setting the Weight
        // foreach (var objective in _tcpObjectives)
        // {
        //     objective.SetWeight(0);
        // }
    }

    private void EnableIKObjectives()
    {
        // if(_tcpSegment == null)
        //     return;
        //
        // // Enable IK by setting the Weight
        // foreach (var objective in _tcpObjectives)
        // {
        //     objective.SetWeight(1);
        // }
        //
        // // Move the TCP Target on top of the TCP Segment
        // _tcpTarget.SetPositionAndRotation(_tcpSegment.Transform.position, _tcpSegment.transform.rotation);
    }

    // Rotates a IK joint towards a positive/negative direction
    public void RotateJointToDirection(JointTransformAndAxis jointInfo, int direction)
    {
        // TODO: Move the TCP target with TCP when Objectives are disabled
        // tcpTarget.SetPositionAndRotation(tcpSegment.Transform.position, tcpSegment.Transform.rotation) ;

        // Check if transform is part of the IK system
        // var rotatingJoint = _bioIK.FindSegment(jointInfo.JointTransform);
        // if (rotatingJoint)
        // {
        //     // Rotate joint by degree amount
        //     if (jointInfo.EnabledRotationAxis == Vector3.right)
        //     {
        //         rotatingJoint.Joint.X.TargetValue += direction * movementSpeed;
        //     } 
        //     else if(jointInfo.EnabledRotationAxis == Vector3.up)
        //     {
        //         rotatingJoint.Joint.Y.TargetValue += direction * movementSpeed;
        //     }
        //     else if(jointInfo.EnabledRotationAxis == Vector3.forward)
        //     {
        //         rotatingJoint.Joint.Z.TargetValue += direction * movementSpeed;
        //     }
        //     else
        //     {       
        //         Debug.LogWarning("Invalid motion provided for rotation");
        //     }
        // }
    }

    public void RotateJointToAngle(JointTransformAndAxis jointInfo, float angle)
    {
        // var rotatingJoint = _bioIK.FindSegment(jointInfo.JointTransform);
        // if (rotatingJoint)
        // {
        //     // Rotate joint to angle amount
        //     if (jointInfo.EnabledRotationAxis == Vector3.right)
        //     {
        //         rotatingJoint.Joint.X.TargetValue += angle;
        //     } 
        //     else if(jointInfo.EnabledRotationAxis == Vector3.up)
        //     {
        //         rotatingJoint.Joint.Z.TargetValue = angle;
        //     }
        //     else if(jointInfo.EnabledRotationAxis == Vector3.forward)
        //     {
        //         rotatingJoint.Joint.Z.TargetValue = angle;
        //     }
        //     else
        //     {       
        //         Debug.LogWarning("Invalid motion provided for rotation");
        //     }
        // }
    }
}