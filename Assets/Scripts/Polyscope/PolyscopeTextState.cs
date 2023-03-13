using Robots.Samples.Unity;
using TMPro;
using UnityEngine;

public class PolyscopeTextState : MonoBehaviour
{
    public string offStateText = "Power off";
    public string idleStateText = "Idle";
    public string normalStateText = "Normal";
    
    private TMP_Text _text;
    
    private void Awake()
    {
        if(TryGetComponent(out _text))
        {
            RobotActions.OnRobotStateChanged += OnRobotStateChanged;
        }
    }

    private void OnRobotStateChanged(Robot.RobotState newState)
    {
        switch (newState)
        {
            case Robot.RobotState.Off:
                _text.text = offStateText;
                break;
            case Robot.RobotState.Idle:
                _text.text = idleStateText;
                break;
            case Robot.RobotState.Normal:
                _text.text = normalStateText;
                break;
                
        }
    }
}
