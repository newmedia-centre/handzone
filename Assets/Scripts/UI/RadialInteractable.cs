using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

[ExecuteInEditMode]
public class RadialInteractable : MonoBehaviour
{
    [Header("UI Control")] 
    public Color selectedColor = Color.cyan;
    public Color circleColor = Color.white;
    public Color arrowTargetColor = Color.gray;
    [Range(0, 360)] public float arrowOrigin = 0f;
    [Range(0, 360)] public float arrowTarget = 90f;
    
    [Header("Haptic Feedback")]
    [Range(0, 1)]
    public float intensity = 0.2f;
    public float duration = 0.2f;

    [Header("Image References")]
    [SerializeField] private Image selectedBackgroundImage;
    [SerializeField] private Image arrowOriginImage;
    [SerializeField] private Image arrowTargetImage;
    [SerializeField] private Image circleImage;

    private float _lastHapticAngle = 0f;

    [FormerlySerializedAs("_interactable")]
    [Header("XR Interactable")]
    [SerializeField] private XRBaseInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        
        selectedBackgroundImage.color = selectedColor;
        arrowOriginImage.color = selectedColor;
        arrowTargetImage.color = arrowTargetColor;
        circleImage.color = circleColor;

        CheckImageReferences();
    }

    void Update()
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

        selectedBackgroundImage.transform.rotation = arrowOriginImage.transform.rotation;

        float originZ = arrowOriginImage.transform.localEulerAngles.z;
        float targetZ = arrowTargetImage.transform.localEulerAngles.z;

        float angle = targetZ - originZ;
        if (angle < 0) angle += 360;

        selectedBackgroundImage.fillAmount = angle / 360f;

        if (Mathf.Approximately(angle % 90, 0) && Mathf.Abs(angle - _lastHapticAngle) > Mathf.Epsilon)
        {
            TriggerHapticFeedback();
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
                XRBaseController controller = controllerInteractor.xrController;
                controller.SendHapticImpulse(intensity, duration);
                Debug.Log("RadialInteractable: Triggered haptic feedback!");
            }
        }
    }
    
    public void SetArrowOrigin(float angle)
    {
        arrowOriginImage.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
    
    public void SetArrowTarget(float angle)
    {
        arrowTargetImage.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
    
    // On editor update
    void OnValidate()
    {
        selectedBackgroundImage.color = selectedColor;
        arrowOriginImage.color = selectedColor;
        arrowTargetImage.color = arrowTargetColor;
        circleImage.color = circleColor;

    }
}
