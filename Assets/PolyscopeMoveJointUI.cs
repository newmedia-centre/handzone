using System.Collections.Generic;
using UnityEngine;

public class PolyscopeMoveJointUI : MonoBehaviour
{
    public List<PolyscopeMovableJoints.MovableJoint> movableJoints;
    public GameObject moveJointElementPrefab;
    public float elementOffset = -25.0f;

    private List<GameObject> _elements;

    private void Awake()
    {
        _elements = new List<GameObject>();
        var offset = Vector3.zero;
        for (int i = 0; i < movableJoints.Count; i++)
        {
            var element = Instantiate(moveJointElementPrefab, transform);
            offset.y = elementOffset * i;
            element.transform.Translate( offset);
            var comp = element.GetComponent<PolyscopeMoveJointElement>();
            comp.jointTarget = movableJoints[i].movableJoint;
            comp.rotateAxis = movableJoints[i].rotateAxis;
            _elements.Add(element);
        }
    }
}
