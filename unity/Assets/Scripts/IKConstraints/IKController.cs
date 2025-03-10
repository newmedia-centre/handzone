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

#endregion

/// <summary>
/// The IKController class manages the inverse kinematics for a robotic arm.
/// It calculates the necessary joint angles to position the arm's end effector
/// at a specified target position. The class handles the initialization of joint
/// angles, performs forward and inverse kinematics calculations, and updates
/// the arm's joint rotations based on the calculated angles.
/// </summary>
public class IKController : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    public ArmJoint[] Joints;
    public float[] Angles;

    public float SamplingDistance = 5f;
    public float LearningRate = 100f;
    public float DistanceThreshold = 0.01f;

    /// <summary>
    /// Initializes the joint angles based on the current local rotations of the joints.
    /// </summary>
    private void Start()
    {
        var angles = new float[Joints.Length];

        for (var i = 0; i < Joints.Length; i++)
            if (Joints[i]._rotationAxis == 'x')
                angles[i] = Joints[i].transform.localRotation.eulerAngles.x;
            else if (Joints[i]._rotationAxis == 'y')
                angles[i] = Joints[i].transform.localRotation.eulerAngles.y;
            else if (Joints[i]._rotationAxis == 'z') angles[i] = Joints[i].transform.localRotation.eulerAngles.z;
        Angles = angles;
    }

    /// <summary>
    /// Updates the inverse kinematics calculations each frame to position the arm's end effector
    /// at the target position.
    /// </summary>
    private void Update()
    {
        InverseKinematics(_targetTransform.position, Angles);
    }

    /// <summary>
    /// Performs forward kinematics calculations to determine the position of the end effector
    /// based on the current joint angles.
    /// </summary>
    /// <param name="angles">An array of joint angles in degrees.</param>
    /// <returns>The position of the end effector as a Vector3.</returns>
    public Vector3 ForwardKinematics(float[] angles)
    {
        var prevPoint = Joints[0].transform.position;
        var rotation = Quaternion.identity;
        for (var i = 1; i < Joints.Length; i++)
        {
            // Rotates around a new axis
            rotation *= Quaternion.AngleAxis(angles[i - 1], Joints[i - 1].RotationAxis);
            var nextPoint = prevPoint + rotation * Joints[i].StartOffset;

            prevPoint = nextPoint;
        }

        return prevPoint;
    }

    /// <summary>
    /// Calculates the distance from the end effector to the target position.
    /// </summary>
    /// <param name="target">The target position as a Vector3.</param>
    /// <param name="angles">An array of joint angles in degrees.</param>
    /// <returns>The distance from the end effector to the target position.</returns>
    public float DistanceFromTarget(Vector3 target, float[] angles)
    {
        var point = ForwardKinematics(angles);
        return Vector3.Distance(point, target);
    }

    /// <summary>
    /// Calculates the partial gradient for a specific joint angle to assist in the inverse
    /// kinematics calculations.
    /// </summary>
    /// <param name="target">The target position as a Vector3.</param>
    /// <param name="angles">An array of joint angles in degrees.</param>
    /// <param name="i">The index of the joint angle to calculate the gradient for.</param>
    /// <returns>The calculated gradient for the specified joint angle.</returns>
    public float PartialGradient(Vector3 target, float[] angles, int i)
    {
        // Saves the angle,
        // it will be restored later
        var angle = angles[i];

        // Gradient : [F(x+SamplingDistance) - F(x)] / h
        var f_x = DistanceFromTarget(target, angles);

        angles[i] += SamplingDistance;
        var f_x_plus_d = DistanceFromTarget(target, angles);

        var gradient = (f_x_plus_d - f_x) / SamplingDistance;

        // Restores
        angles[i] = angle;

        return gradient;
    }

    /// <summary>
    /// Performs inverse kinematics calculations to adjust the joint angles
    /// in order to position the end effector at the target position.
    /// </summary>
    /// <param name="target">The target position as a Vector3.</param>
    /// <param name="angles">An array of joint angles in degrees.</param>
    public void InverseKinematics(Vector3 target, float[] angles)
    {
        if (DistanceFromTarget(target, angles) < DistanceThreshold)
            return;

        for (var i = Joints.Length - 1; i >= 0; i--)
        {
            // Gradient descent
            // Update : Solution -= LearningRate * Gradient
            var gradient = PartialGradient(target, angles, i);
            angles[i] -= LearningRate * gradient;

            // Early termination
            if (DistanceFromTarget(target, angles) < DistanceThreshold)
                return;

            switch (Joints[i]._rotationAxis)
            {
                case 'x':
                    Joints[i].transform.localEulerAngles = new Vector3(angles[i], 0, 0);
                    break;
                case 'y':
                    Joints[i].transform.localEulerAngles = new Vector3(0, angles[i], 0);
                    break;
                case 'z':
                    Joints[i].transform.localEulerAngles = new Vector3(0, 0, angles[i]);
                    break;
            }
        }
    }
}