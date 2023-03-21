using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PolyscopeMovableJoints.asset", menuName = "Polyscope/Movable Joints Object")]
public class PolyscopeMovableJoints : ScriptableObject
{
    [Serializable]
    public class MovableJoint
    {
        public Transform movableJoint;
        public Vector3 rotateAxis;
    }

    public MovableJoint[] movableJoints;    
}
