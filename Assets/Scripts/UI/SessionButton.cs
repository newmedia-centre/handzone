using Schema.Socket.Index;
using UnityEngine;

public class SessionButton : MonoBehaviour
{
    public void SetButton(RobotSession session)
    {
        // Set button text to session name
        transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = session.Address;
    }
}
