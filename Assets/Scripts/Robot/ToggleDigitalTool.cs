using UnityEngine;

public class ToggleDigitalTool : MonoBehaviour
{
    private bool _digitalOutput;

    public void ToggleToolDigitalOut()
    {
        _digitalOutput = RobotClient.Instance.ToggleToolDigitalOut(_digitalOutput);
    }
}
