using UnityEngine;

public class TargetPointsConstraint : Constraint
{
    public Vector3[] targets;
    
    public TargetPointsConstraint(int index) : base(index)
    {
        
    }
}
