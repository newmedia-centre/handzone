// Copyright 2024 NewMedia Centre - Delft University of Technology
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#region

using System;
using System.Collections.Generic;
using System.Globalization;
using Schema.Socket.Index;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

/// <summary>
/// Manages the session menu, including displaying available sessions and handling user interactions.
/// </summary>
public class SessionMenu : MonoBehaviour
{
    public GameObject sessionButtonPrefab;
    public GameObject sessionPlayerNamePrefab;
    public Button joinSessionButton;
    public Button createSessionButton;

    public static Action<string> OnSessionSelected;

    private List<SessionButton> _sessionButtons = new();
    private string _selectedSessionAddress;
    private TextMeshProUGUI _sessionAvailabilityLabel;
    private GameObject sessionsGroup;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the session menu and subscribes to events.
    /// </summary>
    private void Start()
    {
        if (GlobalClient.Instance == null)
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
        transform.Find("SessionPanel/Buttons/JoinButton").TryGetComponent(out joinSessionButton);
        joinSessionButton.interactable = false;
        joinSessionButton.onClick.AddListener(() => { GlobalClient.Instance.JoinSession(_selectedSessionAddress); });

        // Init the create session button
        transform.Find("SessionPanel/Buttons/CreateButton").TryGetComponent(out createSessionButton);
        createSessionButton.interactable = false;
        createSessionButton.onClick.AddListener(() => { GlobalClient.Instance.RequestVirtual(); });

        if (GlobalClient.Instance?.Sessions != null)
        {
            Debug.Log("Updating available sessions in menu...");
            UpdateMenu(GlobalClient.Instance.Sessions);
        }
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Initializes the session availability label.
    /// </summary>
    private void OnEnable()
    {
        // Init the session availability group
        if (_sessionAvailabilityLabel == null)
            _sessionAvailabilityLabel = transform.Find("SessionPanel/AvailabilityGroup/AvailabilityCapacityLabel")
                .GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Called when the object is destroyed.
    /// Unsubscribes from events to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        if (GlobalClient.Instance)
            GlobalClient.Instance.OnSessionsReceived -= UpdateMenu;
    }

    /// <summary>
    /// Updates the session menu with the received sessions.
    /// </summary>
    /// <param name="receivedSessions">The sessions received from the server.</param>
    private void UpdateMenu(SessionsOut receivedSessions)
    {
        // Clear existing session buttons
        foreach (var sessionButton in _sessionButtons) Destroy(sessionButton.gameObject);
        _sessionButtons.Clear();

        // Update the session capacity label
        _sessionAvailabilityLabel.text = receivedSessions.Capacity.ToString(CultureInfo.CurrentCulture);

        // Make create session button interactable if capacity is not full
        if (receivedSessions.Capacity > 0 && createSessionButton)
        {
            if (createSessionButton.interactable == false)
                createSessionButton.interactable = true;
        }
        else
        {
            createSessionButton.interactable = false;
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
    /// <param name="selectedSessionAddress">The address of the selected session.</param>
    private void SetSelectedSession(string selectedSessionAddress)
    {
        // Make the join session button interactable
        if (joinSessionButton.interactable == false)
            joinSessionButton.interactable = true;

        // Deselect every other session button
        foreach (var sessionButton in _sessionButtons) sessionButton.Deselect();

        // Set the selected session address
        _selectedSessionAddress = selectedSessionAddress;
    }
}