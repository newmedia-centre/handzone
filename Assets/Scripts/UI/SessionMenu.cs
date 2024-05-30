using System;
using System.Collections.Generic;
using Schema.Socket.Index;
using UnityEngine;
using UnityEngine.UI;

public class SessionMenu : MonoBehaviour
{
    public GameObject sessionButtonPrefab;
    public GameObject sessionPlayerNamePrefab;

    public static Action<string> OnSessionSelected; 
    
    private List<SessionButton> _sessionButtons = new();
    private string _selectedSessionAddress;
    private Button _joinSessionButton;
    private Button _createSessionButton;

    // Start is called before the first frame update
    void Start()
    {
        if(GlobalClient.Instance != null)
        {
            return;
        }
        
        // Subscribe to the sessions received event
        GlobalClient.Instance.OnSessionsReceived += UpdateMenu;
        
        // Subscribe to the session selected event
        OnSessionSelected += SetSelectedSession;
        
        // Init the join session button
        _joinSessionButton = transform.Find("SessionPanel/Buttons/JoinButton").GetComponent<Button>();
        _joinSessionButton.interactable = false;
        _joinSessionButton.onClick.AddListener(() =>
        {
            GlobalClient.Instance?.JoinSession(_selectedSessionAddress);
        });
        
        // Init the create session button
        _createSessionButton = transform.Find("SessionPanel/Buttons/CreateButton").GetComponent<Button>();
        _createSessionButton.interactable = false;
        _createSessionButton.onClick.AddListener(() =>
        {
            GlobalClient.Instance?.RequestVirtual();
        });
    }

    void OnDestroy()
    {
        GlobalClient.Instance.OnSessionsReceived -= UpdateMenu;
    }

    /// <summary>
    /// Updates the session menu with the received sessions.
    /// </summary>
    /// <param name="receivedSessions"></param>
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
        
        // Make create session button interactable if capacity is not full
        if (receivedSessions.Capacity > 0)
        {
            if(_createSessionButton.interactable == false)
                _createSessionButton.interactable = true;
        }
        else
        {
            _createSessionButton.interactable = false;
        }
        
        // Create new session buttons
        foreach (var receivedSession in receivedSessions.Sessions)
        {
            var sessionsGroup = transform.Find("SessionPanel/SessionsGroup").gameObject;
            var sessionButton = Instantiate(sessionButtonPrefab, sessionsGroup.transform).GetComponent<SessionButton>();
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
        }
    }
    
    /// <summary>
    /// Sets the selected session address and deselects every other session button.
    /// </summary>
    /// <param name="selectedSessionAddress"></param>
    private void SetSelectedSession(string selectedSessionAddress)
    {
        // Make the join session button interactable
        if(_joinSessionButton.interactable == false)
            _joinSessionButton.interactable = true;
        
        // Deselect every other session button
        foreach (var sessionButton in _sessionButtons)
        {
            sessionButton.Deselect();
        }
        
        // Set the selected session address
        _selectedSessionAddress = selectedSessionAddress;
    }
}
