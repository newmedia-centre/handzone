using UnityEngine;
using UnityEngine.EventSystems;

public class PolyscopeMoveTcpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    public TCPController tcpController;
    public Vector3 translateDirection;
    public Vector3 rotateAxis;
    public float accelerationSpeed = 0.15f;
    public float timeSpeed = 0.03f;

    private bool _isHeld;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isHeld = true;
        UR_EthernetIPClient.UpdateSpeedl?.Invoke(translateDirection, rotateAxis, accelerationSpeed, timeSpeed);
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
        }
    }
}
