using Robots.Samples.Unity;
using UnityEngine;

public class PolyscopeState : MonoBehaviour
{
    [SerializeReference]
    private RobotProgram.RobotState state;
    public void ChangeState(PolyscopeState newState)
    {
        RobotProgram.State = newState.state;
        RobotActions.OnRobotStateChanged?.Invoke(newState.state);
    }
}
