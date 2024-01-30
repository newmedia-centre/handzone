using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

[ExecuteInEditMode]
public class RadialInteractable : MonoBehaviour
{
    [Header("UI Control")] 
    public Color selectedColor = Color.cyan;
    public Color arrowOriginColor = Color.cyan;
    public Color circleColor = Color.white;
    public Color arrowTargetColor = Color.gray;
    [Range(0, 360)] public float arrowOrigin = 0f;
    [Range(-360, 360)] public float arrowTarget = 90f;
    
    [Header("Haptic Feedback")]
    [Range(0, 1)]
    public float intensity = 0.2f;
    public float duration = 0.2f;

    [Header("Image References")]
    [SerializeField] private Image selectedBackgroundImage;
    [SerializeField] private Image arrowOriginImage;
    [SerializeField] private Image arrowTargetImage;
    [SerializeField] private Image circleImage;


    [Header("XR Interactable")]
    [SerializeField] private XRBaseInteractable interactable;
    
    private float _lastHapticAngle = 0f;
    private Canvas _canvas;
    private Vector3 _originPosition;

    void Awake()
    {
        interactable = GetComponentInParent<XRBaseInteractable>();
        _canvas = GetComponentInChildren<Canvas>();
        _canvas.enabled = false;
        
        interactable.hoverEntered.AddListener(ShowCanvas);
        interactable.hoverExited.AddListener(HideCanvas);
        interactable.selectExited.AddListener(HideCanvas);
        interactable.selectEntered.AddListener(SetOriginPosition);
        
        UpdateRadial();
        CheckImageReferences();
    }

    void Update()
    {
        // Change the target angle to the offset position of the controller from the starting interactable position
        if (interactable.isSelected)
        {
            if (interactable.GetOldestInteractorSelecting() is XRBaseControllerInteractor controllerInteractor)
            {
                // Get the angle from the origin position to the controller position on the XZ plane
                Vector3 originPosition = new Vector3(_originPosition.x, 0, _originPosition.z);
                Vector3 controllerPosition = new Vector3(controllerInteractor.transform.position.x, 0, controllerInteractor.transform.position.z);
                Vector3 originToController = controllerPosition - originPosition;
                float angle = Vector3.SignedAngle(Vector3.forward, originToController, Vector3.up);
                arrowTarget = angle;
            }
        }
    }

    void LateUpdate()
    {
        if (interactable == null)
        {
            Debug.LogError("RadialInteractable: XRBaseInteractable component not found!");
            return;
        }

        if (arrowOriginImage == null || arrowTargetImage == null || selectedBackgroundImage == null)
        {
            Debug.LogError("RadialInteractable: One or more images are not set!");
            return;
        }
        
        float direction = Mathf.Sign((arrowTarget + arrowOrigin) - arrowOrigin);
        float angle = (arrowTarget + arrowOrigin) - arrowOrigin;
        angle = angle < 0 ? angle + 360 : angle;
        
        selectedBackgroundImage.fillClockwise = direction < 0;
        selectedBackgroundImage.fillAmount = angle / 360f;
        selectedBackgroundImage.transform.rotation = arrowOriginImage.transform.rotation;

        if (angle % 90 == 0 && angle != _lastHapticAngle)
        {
            // TriggerHapticFeedback();
            _lastHapticAngle = angle;
        }
    }

    private void CheckImageReferences()
    {
        if (arrowOriginImage == null || arrowTargetImage == null || selectedBackgroundImage == null)
        {
            Debug.LogError("RadialInteractable: One or more images are not set!");
            enabled = false;
        }
    }

    private void TriggerHapticFeedback()
    {
        if (intensity > 0)
        {
            XRBaseControllerInteractor controllerInteractor = GetComponent<XRBaseControllerInteractor>();
            if (controllerInteractor != null)
            {
                controllerInteractor.SendHapticImpulse(intensity, duration);
                Debug.Log("RadialInteractable: Triggered haptic feedback!");
            }
        }
    }
    
    private void ShowCanvas(BaseInteractionEventArgs args)
    {
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            _canvas.enabled = true;
        }
    }
    
    private void HideCanvas(BaseInteractionEventArgs args)
    {
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            // It should hide if the controller is not hovering anymore and not selected
            if (!interactable.isSelected)
                _canvas.enabled = false;
        }
    }
    
    private void SetOriginPosition(BaseInteractionEventArgs args)
    {
        // Set the origin position to the controller position on select
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            _originPosition = controllerInteractor.transform.position;
        }
    }

    // On editor update
    void OnValidate()
    {
        UpdateRadial();
    }

    void UpdateRadial()
    {
        arrowOriginImage.transform.localEulerAngles = new Vector3(0, 0, arrowOrigin);
        arrowTargetImage.transform.localEulerAngles = new Vector3(0, 0, arrowTarget + arrowOrigin);
        selectedBackgroundImage.color = selectedColor;
        arrowOriginImage.color = arrowOriginColor;
        arrowTargetImage.color = arrowTargetColor;
        circleImage.color = circleColor;
    }
}
