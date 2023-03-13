using Robots.Samples.Unity;
using UnityEngine;

public class StateDebugger : MonoBehaviour
{
    public Robot.RobotState State = Robot.RobotState.Off;

    private Robot.RobotState _currentState;

    private void Start()
    {
        _currentState = State;
        RobotActions.OnRobotStateChanged?.Invoke(_currentState);
    }
    
    private void Update()
    {
        if (State != _currentState) {
            _currentState = State;
            RobotActions.OnRobotStateChanged?.Invoke(_currentState);
        }
    }
}
