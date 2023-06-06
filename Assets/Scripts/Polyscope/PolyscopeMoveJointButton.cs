using UnityEngine;
using UnityEngine.EventSystems;

public class PolyscopeMoveJointButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int jointIndex;
    public int direction;
    private bool _isHeld;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _isHeld = true;
        PolyscopeRobot.DisableIK?.Invoke();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        _isHeld = false;
        PolyscopeRobot.EnableIK?.Invoke();
        UR_EthernetIPClient.ClearSendBuffer();
    }

    private void Update()
    {
        if (_isHeld)
        {
            RobotTranslator.MovePolyscopeJoint?.Invoke(jointIndex, direction);
        }
    }
}
