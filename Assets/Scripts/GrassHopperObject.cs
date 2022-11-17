using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Outline))]
public class GrassHopperObject : MonoBehaviour
{
    public bool sendPosition = true;
    public bool sendRotation = true;
    public float updateDuration = 3;
    public Color selectableColor = Color.green;
    public Color unselectableColor = Color.red;
    public Color interactedColor = Color.cyan;

    private const int SCALE = 1000;
    private Timer _timer;
    private bool _shouldUpdate = false;
    private XRGrabInteractable _xrInteractable;
    private Outline _selectableOutline;
    private Outline _meshOutline;
    private Transform _meshTransform;
    private Rigidbody _rigidbody;
    private bool _previousKinematicSetting;

    private void Awake()
    {
        _timer = GetComponent<Timer>();
        _timer.SetTimerDuration(updateDuration);
        _xrInteractable = GetComponent<XRGrabInteractable>();
        _rigidbody = GetComponent<Rigidbody>();
        _selectableOutline = GetComponent<Outline>();
        _selectableOutline.OutlineColor = selectableColor;
        
        _meshTransform = transform.GetChild(0);
        _meshOutline = _meshTransform.GetComponent<Outline>();
        _meshOutline.OutlineColor = selectableColor;
    }

    private void OnEnable()
    {
        RobotActions.OnToolGrabbed += MeshUnselectable;
        RobotActions.OnToolUngrabbed += MeshSelectable;
        _xrInteractable.selectEntered.AddListener(delegate
        {
            ResetMesh(); 
            ShouldUpdate(true);
        });
        _xrInteractable.selectExited.AddListener(delegate
        {
            ShouldUpdate(false);
        });
    }

    private void Update()
    {
        if (transform.hasChanged && !_timer.Started() && _shouldUpdate)
        {
            if (!sendPosition && !sendRotation)
                return;
                
            if (sendPosition)
            {
                UnityInGrasshopper.Instance.SendPosition(transform.position * SCALE, name);
            }
            if (sendRotation)
            {
                UnityInGrasshopper.Instance.SendRotationQuaternion(transform.rotation, name);
            }

            transform.hasChanged = false;
            _timer.StartTimer();
        }
    }

    public void ShouldUpdate(bool value)
    {
        _shouldUpdate = value;
    }

    public void MeshUnselectable(GameObject go)
    {
        if (go == _meshTransform.gameObject)
        {
            _meshOutline.OutlineColor = unselectableColor;
            _selectableOutline.OutlineColor = interactedColor;
            _rigidbody.isKinematic = _previousKinematicSetting;
        }
    }

    public void MeshSelectable(GameObject go)
    {
        if (go == _meshTransform.gameObject)
        {
            _meshOutline.OutlineColor = selectableColor;
            _selectableOutline.OutlineColor = selectableColor;
            _rigidbody.isKinematic = _previousKinematicSetting;
            if (_rigidbody.isKinematic)
            {
                _previousKinematicSetting = true;
            }
            else
            {
                _previousKinematicSetting = false;
                _rigidbody.isKinematic = true;
            }
        }
    }
    
    void ResetMesh()
    {
        _meshTransform.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
