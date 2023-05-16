using Robots.Samples.Unity;
using UnityEngine;

public class StateDebugger : MonoBehaviour
{
    public RobotProgram.RobotState State = RobotProgram.RobotState.Off;

    private RobotProgram.RobotState _currentState;

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
