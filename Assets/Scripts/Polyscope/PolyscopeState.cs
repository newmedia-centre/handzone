using Robots.Samples.Unity;
using UnityEngine;

public class PolyscopeState : MonoBehaviour
{
    [SerializeReference]
    private Robot.RobotState state;
    public void ChangeState(PolyscopeState newState)
    {
        Robot.State = newState.state;
        RobotActions.OnRobotStateChanged?.Invoke(newState.state);
    }
}
