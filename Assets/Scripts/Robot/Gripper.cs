using UnityEngine;

public class Gripper : MonoBehaviour
{
    public GameObject grabbableObject;
    public GameObject currentGrabbedObject;

    private bool _isGrabbing;
    private GameObject _attachAnchor;
    private bool previousKinematicSetting;

    private void Awake()
    {
        _attachAnchor = new GameObject(name + "Attach Anchor");
        _attachAnchor.transform.SetParent(transform);
    }

    private void OnEnable()
    {
        RobotActions.OnGripperLoaded += OnGripperLoaded;
        RobotActions.OnGripperUnloaded += OnGripperUnloaded;
    }

    private void OnDisable()
    {
        RobotActions.OnGripperLoaded -= OnGripperLoaded;
        RobotActions.OnGripperUnloaded -= OnGripperUnloaded;
    }

    private void OnTriggerEnter(Collider other)
    {
        grabbableObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == grabbableObject)
        {
            grabbableObject = null;
        }
    }

    private void OnGripperUnloaded()
    {
        _isGrabbing = false;

        if (currentGrabbedObject)
        {
            currentGrabbedObject.GetComponent<Rigidbody>().isKinematic = previousKinematicSetting;
            currentGrabbedObject = null;
        }
    }

    void OnGripperLoaded()
    {
        if (grabbableObject)
        {
            currentGrabbedObject = grabbableObject;
            var grabbedObjectRigidbody = currentGrabbedObject.GetComponent<Rigidbody>();
            if (grabbedObjectRigidbody.isKinematic)
            {
                previousKinematicSetting = true;
            }
            else
            {
                previousKinematicSetting = false;
                grabbedObjectRigidbody.isKinematic = true;
            }
      
            grabbableObject = null;
            _isGrabbing = true;
        }
    }

    private void Update()
    {
        if (_isGrabbing)
        {
            currentGrabbedObject.transform.SetPositionAndRotation(_attachAnchor.transform.position, _attachAnchor.transform.rotation);
        }
    }

    public void SetAnchorPosition(Vector3 position)
    {
        _attachAnchor.transform.localPosition = position;
    }
}
