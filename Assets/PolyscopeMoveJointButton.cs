using UnityEngine;
using UnityEngine.EventSystems;

public class PolyscopeMoveJointButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Transform jointToMove;
    public Vector3 rotateAxis;
    private bool _isHeld;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _isHeld = true;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        _isHeld = false;
    }

    private void Update()
    {
        if (_isHeld)
        {
            PolyscopeRobot.OnPolyscopeRotateJointToDirection(jointToMove, rotateAxis );
        }
    }
}
