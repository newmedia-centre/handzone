using Robots.Samples.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PolyscopeIconState : MonoBehaviour
{
    public Color offStateColor = Color.red;
    public Color idleStateColor = Color.yellow;
    public Color normalStateColor = Color.green;
    
    private Image _icon;

    private void Awake()
    {
        if(TryGetComponent(out _icon))
        {
            _icon.color = Color.black;
            RobotActions.OnRobotStateChanged += OnRobotStateChanged;
        }
    }

    private void OnRobotStateChanged(Robot.RobotState newState)
    {
        switch (newState)
        {
            case Robot.RobotState.Off:
                _icon.color = offStateColor;
                break;
            case Robot.RobotState.Idle:
                _icon.color = idleStateColor;
                break;
            case Robot.RobotState.Normal:
                _icon.color = normalStateColor;
                break;
            default:
                _icon.color = Color.black;
                break;
        }
    }

}
