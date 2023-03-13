using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PolyscopeMovableJoints.asset", menuName = "Polyscope/Movable Joints Object")]
public class PolyscopeMovableJoints : ScriptableObject
{
    [Serializable]
    public class MovableJoint
    {
        public Transform movableJoint;
        [FormerlySerializedAs("axisDirection")] public Vector3 rotateAxis;
    }

    public MovableJoint[] movableJoints;    
}
