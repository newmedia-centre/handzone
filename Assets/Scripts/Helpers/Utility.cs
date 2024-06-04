using System.Collections;
using System.Collections.Generic;
using Schema.Socket.Unity;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return RotatePointAroundPivot(point, pivot, Quaternion.Euler(angles));
    }
 
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) {
        return rotation * (point - pivot) + pivot;
    }
    
    public static SixDofPosition TransformToSixDofPosition(Transform transform)
    {
        return new SixDofPosition
        {
            Position = new Vector3D
            {
                X = transform.position.x,
                Y = transform.position.y,
                Z = transform.position.z
            },
            Rotation = new Vector3D
            {
                X = transform.rotation.eulerAngles.x,
                Y = transform.rotation.eulerAngles.y,
                Z = transform.rotation.eulerAngles.z
            }
        };
    }
    
    public static void SixDofPositionToTransform(SixDofPosition sixDofPosition, Transform transform)
    {
        transform.position = new Vector3
        {
            x = (float)sixDofPosition.Position.X,
            y = (float)sixDofPosition.Position.Y,
            z = (float)sixDofPosition.Position.Z
        };
        transform.eulerAngles = new Vector3
        {
            x = (float)sixDofPosition.Rotation.X,
            y = (float)sixDofPosition.Rotation.Y,
            z = (float)sixDofPosition.Rotation.Z
        };
    }
}
