using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PolyscopeMoveJointElement : MonoBehaviour
{
    public Button positiveButton;
    public Button negativeButton;
    public Transform jointTarget;
    public Vector3 rotateAxis;
    
    TMP_Text _jointLabel;
    Slider _slider;
    TMP_InputField _inputField;

    private void Start()
    {
        if (jointTarget == null)
        {
            Debug.LogError("Joint target not assigned to component.");
            return;
        }
        
        _slider = GetComponentInChildren<Slider>();
        _inputField = GetComponentInChildren<TMP_InputField>();
        _jointLabel = GetComponent<TMP_Text>();
        _jointLabel.text = jointTarget.name;

        _inputField.onSubmit.AddListener(delegate { UpdateJoint(); });
        positiveButton.onClick.AddListener(delegate { UpdateUI(); });
        negativeButton.onClick.AddListener(delegate { UpdateUI(); });
        
        // Add components to buttons and set their initial values
        var comp = positiveButton.gameObject.AddComponent(typeof(PolyscopeMoveJointButton)) as PolyscopeMoveJointButton;
        if (comp != null)
        {
            comp.rotateAxis = rotateAxis;
            comp.jointToMove = jointTarget;
        }
        
        // Change negative direction Vector to inverse for only this button
        comp = negativeButton.gameObject.AddComponent(typeof(PolyscopeMoveJointButton)) as PolyscopeMoveJointButton;
        if (comp != null)
        {
            comp.rotateAxis = -rotateAxis;
            comp.jointToMove = jointTarget;
        }
    }

    private void UpdateUI()
    {
        var localRotation = jointTarget.localRotation.eulerAngles.magnitude.ToString();
        _inputField.text = localRotation;
        _slider.value = float.Parse(localRotation);
    }

    void UpdateJoint()
    {
        if (float.TryParse(_inputField.text, out var angle))
        {
            _slider.value = angle;
            PolyscopeRobot.OnPolyscopeRotateJointToAngle(jointTarget, angle, rotateAxis);
        }
    }
}
