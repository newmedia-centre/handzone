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

    private void OnRobotStateChanged(RobotProgram.RobotState newState)
    {
        switch (newState)
        {
            case RobotProgram.RobotState.Off:
                _text.text = offStateText;
                break;
            case RobotProgram.RobotState.Idle:
                _text.text = idleStateText;
                break;
            case RobotProgram.RobotState.Normal:
                _text.text = normalStateText;
                break;
                
        }
    }
}
