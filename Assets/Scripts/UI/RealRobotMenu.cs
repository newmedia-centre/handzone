using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RealRobotMenu : MonoBehaviour
{
    private Button _joinButton;
    private TMP_Text _statusText;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("SessionPanel/Buttons/JoinButton").TryGetComponent(out _joinButton);
        transform.Find("SessionPanel/SessionsGroup/Status").TryGetComponent(out _statusText);
        
        _joinButton.onClick.AddListener(() =>
        {
            _statusText.text = "Checking availability...";
            GlobalClient.Instance.RequestReal();
        });
        
        GlobalClient.Instance.OnSessionJoinFailed += DisplayError;
    }

    private void DisplayError(string error)
    {
        Debug.LogError(error);
    }
}
