using System.Collections.Generic;
using Schema.Socket.Index;
using UnityEngine;

public class SessionMenu : MonoBehaviour
{
    public GameObject sessionButtonPrefab;
    public GameObject sessionPlayerNamePrefab;
    
    private List<GameObject> _sessionButtons = new();
    private string _selectedSessionAddress;
    private UnityEngine.UI.Button _joinSessionButton;

    // Start is called before the first frame update
    void Start()
    {
        if(GlobalClient.Instance.Sessions != null)
        {
            return;
        }
        
        GlobalClient.Instance.OnSessionsReceived += UpdateMenu;
        _joinSessionButton = transform.Find("SessionPanel/Buttons/JoinButton").GetComponent<UnityEngine.UI.Button>();
        _joinSessionButton.onClick.AddListener(() =>
        {
            SessionClient.Instance?.JoinSession(_selectedSessionAddress);
        });
    }

    void OnDestroy()
    {
        GlobalClient.Instance.OnSessionsReceived -= UpdateMenu;
    }

    private void UpdateMenu(SessionsOut receivedSessions)
    {
        // Clear existing session buttons
        foreach (var sessionButton in _sessionButtons)
        {
            Destroy(sessionButton);
        }
        _sessionButtons.Clear();
        
        if (receivedSessions == null || receivedSessions.Sessions == null)
        {
            return;
        }
        
        // Update the session capacity label
        var _sessionAvailabilityGroup = transform.Find("SessionPanel/AvailabilityGroup").gameObject;
        _sessionAvailabilityGroup.transform.Find("AvailabilityCapacityLabel").GetComponent<TMPro.TextMeshProUGUI>().text = receivedSessions.Capacity.ToString();
        
        // Create new session buttons
        foreach (var receivedSession in receivedSessions.Sessions)
        {
            var sessionsGroup = transform.Find("SessionPanel/SessionsGroup").gameObject;
            var sessionButton = Instantiate(sessionButtonPrefab, sessionsGroup.transform);
            sessionButton.GetComponent<SessionButton>().SetButton(receivedSession);
            _sessionButtons.Add(sessionButton);
            
            // Find the Text tabel in the Session Button to set the session name
            var _sessionName = sessionButton.transform.Find("SessionName").gameObject;
            _sessionName.GetComponent<TMPro.TextMeshProUGUI>().text = receivedSession.Name;
            
            // Find the SessionPlayerNamesGroup object in this object's children
            var _sessionPlayerNamesGroup = sessionButton.transform.Find("UsersPanel").gameObject;
            
            // Fill the session player names to the Session Button
            foreach (var user in receivedSession.Users)
            {
                var sessionPlayerName = Instantiate(sessionPlayerNamePrefab, _sessionPlayerNamesGroup.transform);
                sessionPlayerName.GetComponent<TMPro.TextMeshProUGUI>().text = user;
            }
            
            // Add a listener to the session button to set the selected session address
            sessionButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                _selectedSessionAddress = receivedSession.Address;
                Debug.Log(_selectedSessionAddress);
            });            
        }
    }
}
