using System.Collections.Generic;
using Schema.Socket.Index;
using UnityEngine;

public class SessionMenu : MonoBehaviour
{
    public GameObject sessionButtonPrefab;
    
    private List<GameObject> _sessionButtons = new();
    private string _selectedSessionAddress;
    
    private UnityEngine.UI.Button _joinSessionButton;

    // Start is called before the first frame update
    void Start()
    {
        GlobalClient.Instance.OnRobotsReceived += UpdateMenu;
        _joinSessionButton = transform.GetChild(1).GetComponent<UnityEngine.UI.Button>();
        _joinSessionButton.onClick.AddListener(() =>
        {
            SessionClient.Instance?.JoinSession(_selectedSessionAddress);
        });
    }

    void OnDestroy()
    {
        GlobalClient.Instance.OnRobotsReceived -= UpdateMenu;
    }

    private void UpdateMenu(RobotsOut receivedSessions)
    {
        foreach (var button in _sessionButtons)
        {
            Destroy(button);
        }
        _sessionButtons.Clear();

        if (receivedSessions == null || receivedSessions.Sessions == null)
        {
            return;
        }

        foreach (var session in receivedSessions.Sessions)
        {
            var sessionButton = Instantiate(sessionButtonPrefab, transform);
            sessionButton.GetComponent<SessionButton>().SetButton(session);
            _sessionButtons.Add(sessionButton);
            
            sessionButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                _selectedSessionAddress = session.Address;
            });
        }
    }
}
