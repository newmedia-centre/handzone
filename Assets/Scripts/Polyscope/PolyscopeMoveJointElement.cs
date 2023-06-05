using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PolyscopeMoveJointElement : MonoBehaviour
{
    public Button positiveButton;
    public Button negativeButton;
    public int jointIndex;

    TMP_Text _jointLabel;
    Slider _slider;
    TMP_InputField _inputField;

    private void Start()
    {
        _slider = GetComponentInChildren<Slider>();
        _inputField = GetComponentInChildren<TMP_InputField>();

        _inputField.onSubmit.AddListener(delegate { UpdateJoint(); });
        UR_EthernetIPClient.JointChanged += UpdateUI;
        
        // Add components to buttons and set their initial values
        var moveJointButton = positiveButton.gameObject.AddComponent(typeof(PolyscopeMoveJointButton)) as PolyscopeMoveJointButton;
        if (moveJointButton != null)
        {
            moveJointButton.jointIndex = jointIndex;
            moveJointButton.direction = 1;
        }
        
        // Change negative direction Vector to inverse for only this button
        moveJointButton = negativeButton.gameObject.AddComponent(typeof(PolyscopeMoveJointButton)) as PolyscopeMoveJointButton;
        if (moveJointButton != null)
        {
            moveJointButton.jointIndex = jointIndex;
            moveJointButton.direction = -1;
        }
    }


    private void UpdateUI(int jointIndex, float angle)
    {
        if(jointIndex == this.jointIndex)
        {
            angle *= Mathf.Rad2Deg;
            _slider.value = angle;
            _inputField.text = angle.ToString("F2");
        }
    }

    void UpdateJoint()
    {
        if (float.TryParse(_inputField.text, out var angle))
        {
            _slider.value = angle;
            RobotTranslator.UpdatePolyscopeJoint(jointIndex, angle);
        }
    }
}
