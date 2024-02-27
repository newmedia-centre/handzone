using UnityEngine;
using UnityEngine.EventSystems;

public class PolyscopeMoveTcpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    public TCPController tcpController;
    public Vector3 translateDirection;
    public Vector3 rotateAxis;

    private bool _isHeld;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isHeld = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isHeld = false;
        UR_EthernetIPClient.ClearSendBuffer?.Invoke();
    }

    private void Update()
    {
        if (_isHeld)
        {
            tcpController.TranslateObject(translateDirection);
            tcpController.RotateObject(rotateAxis);
            
            // RobotTranslator.MovePolyscopeTCP?.Invoke(translateDirection, rotateAxis);
            // TODO: CHange to use WebClient event
            WebClient.Instance.Speedl(translateDirection, rotateAxis, 0.1f, 0.1f);
        }
    }
}