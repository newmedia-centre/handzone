using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return RotatePointAroundPivot(point, pivot, Quaternion.Euler(angles));
    }
 
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) {
        return rotation * (point - pivot) + pivot;
    }
}
