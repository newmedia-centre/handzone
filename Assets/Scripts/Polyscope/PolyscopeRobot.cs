using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolyscopeRobot : MonoBehaviour
{
    public float movementSpeed = 3.0f;
    public List<Transform> joints;
    public Transform TCP;
    public static Action<Transform, Vector3> OnPolyscopeRotateJointToDirection;
    public static Action<Transform, float, Vector3> OnPolyscopeRotateJointToAngle;

     void Awake()
     {
         // Get all children transform components to get all the joints
         joints = transform.GetComponentsInChildren<Transform>().Skip(1).ToList();
         
         // Use last joint for the TCP
         TCP = joints.Last();
         // Remove TCP from main Joints
         joints = joints.SkipLast(1).ToList();

         OnPolyscopeRotateJointToDirection += RotateJointToDirection;
         OnPolyscopeRotateJointToAngle += RotateJointToAngle;
     }

     // Moves a joint towards a positive/negative direction
     public void RotateJointToDirection(Transform joint, Vector3 direction)
     {
         // Check if transform is part of the joints
         if (joints.Contains(joint))
         {
             // Rotate joint by degree amount, Rotation Constraint prevents rotating joint to a wrong axis
             joint.transform.localEulerAngles += (direction.normalized * movementSpeed);
         }
     }

     public void RotateJointToAngle(Transform joint, float angle, Vector3 direction)
     {
         if (joints.Contains(joint))
         {
             // var rotation = Quaternion.Euler(direction.normalized * angle);
             var angleDir = direction.normalized * angle;
             joint.transform.localEulerAngles = angleDir;
         }
     }
}