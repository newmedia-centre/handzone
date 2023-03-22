using UnityEngine;
using UnityEngine.EventSystems;

public class PolyscopeMoveJointButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PolyscopeRobot.JointTransformAndAxis jointToMove;
    public int direction;
    private bool _isHeld;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _isHeld = true;
        PolyscopeRobot.DisableIK.Invoke();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        _isHeld = false;
        PolyscopeRobot.EnableIK.Invoke();
    }

    private void Update()
    {
        if (_isHeld)
        {
            PolyscopeRobot.OnPolyscopeRotateJointToDirection(jointToMove, direction );
        }
    }
}
