using UnityEngine;

public class Gripper : MonoBehaviour
{
    public GameObject grabbableObject;
    public GameObject currentGrabbedObject;

    private bool _isGrabbing;
    private GameObject _attachAnchor;
    private bool _previousKinematicSetting;

    private void Awake()
    {
        _attachAnchor = new GameObject(name + "Attach Anchor");
        _attachAnchor.transform.SetParent(transform);
    }

    private void OnEnable()
    {
        RobotActions.OnToolLoaded += Grab;
        RobotActions.OnToolUnloaded += UnGrab;
    }

    private void OnDisable()
    {
        RobotActions.OnToolLoaded -= Grab;
        RobotActions.OnToolUnloaded -= UnGrab;
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

    public void UnGrab()
    {
        _isGrabbing = false;

        if (currentGrabbedObject)
        {
            RobotActions.OnToolUngrabbed(currentGrabbedObject);
            currentGrabbedObject = null;
        }
    }

    public void Grab()
    {
        if (grabbableObject)
        {
            
            currentGrabbedObject = grabbableObject;
            RobotActions.OnToolGrabbed(currentGrabbedObject);

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
