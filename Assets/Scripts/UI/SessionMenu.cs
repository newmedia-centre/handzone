using System;
using System.Collections.Generic;
using System.Globalization;
using Schema.Socket.Index;
using TMPro;
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
    private TextMeshProUGUI _sessionAvailabilityLabel;
    private GameObject sessionsGroup;
    
    // Start is called before the first frame update
    void Start()
    {
        if(GlobalClient.Instance == null)
        {
            Debug.LogError("GlobalClient instance is null. Make sure to have a GlobalClient instance in the scene.");
            return;
        }

        sessionsGroup = transform.Find("SessionPanel/SessionsGroup").gameObject;
        
        // Subscribe to the sessions received event
        GlobalClient.Instance.OnSessionsReceived += UpdateMenu;
        
        // Subscribe to the session selected event
        OnSessionSelected += SetSelectedSession;

        // Init the join session button
        transform.Find("SessionPanel/Buttons/JoinButton").TryGetComponent(out _joinSessionButton);
        _joinSessionButton.interactable = false;
        _joinSessionButton.onClick.AddListener(() =>
        {
            GlobalClient.Instance.JoinSession(_selectedSessionAddress);
        });
        
        // Init the create session button
        transform.Find("SessionPanel/Buttons/CreateButton").TryGetComponent(out _createSessionButton);
        _createSessionButton.interactable = false;
        _createSessionButton.onClick.AddListener(() =>
        {
            GlobalClient.Instance.RequestVirtual();
        });
    }

    private void OnEnable()
    {
        // Init the session availability group
        if(_sessionAvailabilityLabel == null)                
            _sessionAvailabilityLabel = transform.Find("SessionPanel/AvailabilityGroup/AvailabilityCapacityLabel").GetComponent<TextMeshProUGUI>();

        if (GlobalClient.Instance?.Sessions != null)
        {
            Debug.Log("Updating available sessions in menu...");
            UpdateMenu(GlobalClient.Instance.Sessions);
        }
    }

    void OnDestroy()
    {
        if(GlobalClient.Instance)
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
            Destroy(sessionButton.gameObject);
        }
        _sessionButtons.Clear();
        
        // Update the session capacity label
        _sessionAvailabilityLabel.text = receivedSessions.Capacity.ToString(CultureInfo.CurrentCulture);
        
        // Make create session button interactable if capacity is not full
        if (receivedSessions.Capacity > 0 && _createSessionButton)
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
            Debug.Log(receivedSession.Type + " " + GlobalClient.Instance.SessionType);
            // Only create buttons for current selected session type
            if (receivedSession.Type != GlobalClient.Instance.SessionType)
                continue;
            
            var sessionButtonGb = Instantiate(sessionButtonPrefab, sessionsGroup.transform);
            var sessionButton = sessionButtonGb.GetComponent<SessionButton>();
            sessionButton.GetComponent<SessionButton>().SetButton(receivedSession);
            _sessionButtons.Add(sessionButton);
            
            // Find the Text label in the Session Button to set the session name
            var sessionName = sessionButton.transform.Find("SessionName").gameObject;
            sessionName.GetComponent<TextMeshProUGUI>().text = receivedSession.Name;
            
            // Find the SessionPlayerNamesGroup object in this object's children
            var sessionPlayerNamesGroup = sessionButton.transform.Find("UsersPanel").gameObject;
            
            // Fill the session player names to the Session Button
            foreach (var user in receivedSession.Users)
            {
                var sessionPlayerName = Instantiate(sessionPlayerNamePrefab, sessionPlayerNamesGroup.transform);
                sessionPlayerName.GetComponent<TextMeshProUGUI>().text = user;
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
