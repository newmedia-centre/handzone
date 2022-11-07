using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GripperObject : MonoBehaviour
{
    public GameObject grabableObject;
    public GameObject currentGrabbedObject;
    
    private Transform _oldParent;

    private void OnEnable()
    {
        RobotActions.OnGripperLoaded += AttachToParent;
        RobotActions.OnGripperUnloaded += DetachFromParent;
    }

    private void OnDisable()
    {
        RobotActions.OnGripperLoaded -= AttachToParent;
        RobotActions.OnGripperUnloaded -= DetachFromParent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != currentGrabbedObject)
            grabableObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentGrabbedObject)
        {
            DetachFromParent();
        }
        else if (other.gameObject == grabableObject)
        {
            grabableObject = null;
        }

    }

    private void DetachFromParent()
    {
        if (currentGrabbedObject)
        {
            currentGrabbedObject.transform.parent = _oldParent;
            currentGrabbedObject = null;
        }
    }

    void AttachToParent()
    {
        if (grabableObject)
        {
            currentGrabbedObject = grabableObject;
            grabableObject = null;
            _oldParent = currentGrabbedObject.transform.parent;
            currentGrabbedObject.transform.parent = transform;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(currentGrabbedObject == null)
                AttachToParent();
            else
                DetachFromParent();
        }
    }
}
