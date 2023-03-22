using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PolyscopeMoveJointElement : MonoBehaviour
{
    public Button positiveButton;
    public Button negativeButton;
    public PolyscopeRobot.JointTransformAndAxis jointInfo;
    
    TMP_Text _jointLabel;
    Slider _slider;
    TMP_InputField _inputField;

    private void Start()
    {
        
        _slider = GetComponentInChildren<Slider>();
        _inputField = GetComponentInChildren<TMP_InputField>();
        _jointLabel = GetComponent<TMP_Text>();
        
        _jointLabel.text = jointInfo.JointTransform.name;

        _inputField.onSubmit.AddListener(delegate { UpdateJoint(); });
        positiveButton.onClick.AddListener(delegate { UpdateUI(); });
        negativeButton.onClick.AddListener(delegate { UpdateUI(); });
        
        // Add components to buttons and set their initial values
        var moveJointButton = positiveButton.gameObject.AddComponent(typeof(PolyscopeMoveJointButton)) as PolyscopeMoveJointButton;
        if (moveJointButton != null)
        {
            moveJointButton.jointToMove = jointInfo;
            moveJointButton.direction = 1;
        }
        
        // Change negative direction Vector to inverse for only this button
        moveJointButton = negativeButton.gameObject.AddComponent(typeof(PolyscopeMoveJointButton)) as PolyscopeMoveJointButton;
        if (moveJointButton != null)
        {
            moveJointButton.jointToMove = jointInfo;
            moveJointButton.direction = -1;
        }
    }

    private void UpdateUI()
    {
        var localRotation = jointInfo.JointTransform.localRotation.eulerAngles.magnitude.ToString();
        _inputField.text = localRotation;
        _slider.value = float.Parse(localRotation);
    }

    void UpdateJoint()
    {
        if (float.TryParse(_inputField.text, out var angle))
        {
            _slider.value = angle;
            PolyscopeRobot.OnPolyscopeRotateJointToAngle(jointInfo, angle);
        }
    }
}
