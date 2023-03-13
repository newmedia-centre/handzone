using System;
using Robots.Samples.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PolyscopeInitializeUI : MonoBehaviour
{
    public GameObject offButton;
    public GameObject idleButton;
    public GameObject normalButton;
    
    private void Awake()
    {
        if (offButton && idleButton && normalButton)
        {
            RobotActions.OnRobotStateChanged += OnRobotStateChanged;
        }
        else
        {
            Debug.LogWarning("Buttons are not assigned for changed state events.");
        }
    }

    private void OnRobotStateChanged(Robot.RobotState newState)
    {
        switch (newState)
        {
            case Robot.RobotState.Off:
                offButton.GetComponent<Button>().interactable = false;
                idleButton.SetActive(true);
                normalButton.SetActive(false);
                normalButton.GetComponent<Button>().interactable = true;
                break;
            case Robot.RobotState.Idle:
                offButton.GetComponent<Button>().interactable = true;
                idleButton.SetActive(false);
                normalButton.SetActive(true);
                normalButton.GetComponent<Button>().interactable = true;
                break;
            case Robot.RobotState.Normal:
                idleButton.SetActive(false);
                normalButton.SetActive(true);
                normalButton.GetComponent<Button>().interactable = false;
                break;
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }
}
