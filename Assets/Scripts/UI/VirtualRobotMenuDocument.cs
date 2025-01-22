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
using Schema.Socket.Index;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

#endregion

public class VirtualRobotMenuDocument : MonoBehaviour
{
    public UIDocument virtualRobotDocument;

    private ScrollView _sessionsGroupScrollView;
    private Button _joinButton;
    private Button _createButton;
    private Button _backButton;
    private Button _returnButton;
    private List<VisualElement> _sessionButtons = new();
    private string _selectionSessionAddress;

    private void Awake()
    {
        virtualRobotDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _sessionsGroupScrollView = virtualRobotDocument.rootVisualElement.Q<ScrollView>("SessionsGroupScrollView");
        _joinButton = virtualRobotDocument.rootVisualElement.Q<Button>("JoinButton");
        _createButton = virtualRobotDocument.rootVisualElement.Q<Button>("CreateButton");
        _returnButton = virtualRobotDocument.rootVisualElement.Q<Button>("ReturnButton");
        _backButton = virtualRobotDocument.rootVisualElement.Q<Button>("BackButton");

        _joinButton.SetEnabled(false);
        _joinButton.focusable = false;

        _joinButton.clicked += OnJoinClicked;
        _createButton.clicked += OnCreateClicked;
        _backButton.clicked += OnBackClicked;
        _returnButton.clicked += OnBackClicked;

        if (GlobalClient.Instance?.Sessions != null) UpdateMenu(GlobalClient.Instance.Sessions);
    }

    /// <summary>
    /// Called when the object becomes disabled.
    /// Unsubscribes from button click events.
    /// </summary>
    private void OnDisable()
    {
        _joinButton.clicked -= OnJoinClicked;
        _createButton.clicked -= OnCreateClicked;
        _backButton.clicked -= OnBackClicked;
        _returnButton.clicked -= OnBackClicked;
    }

    private void Start()
    {
        GlobalClient.Instance.OnSessionsReceived += UpdateMenu;


        if (GlobalClient.Instance?.Sessions != null) UpdateMenu(GlobalClient.Instance.Sessions);
    }

    private void OnCreateClicked()
    {
        GlobalClient.Instance.RequestVirtual();
        MenuController.Instance.HideMenu();
    }

    private void UpdateMenu(SessionsOut receivedSessions)
    {
        _sessionsGroupScrollView.Clear();
        _sessionButtons.Clear();

        foreach (var receivedSession in receivedSessions.Sessions)
        {
            var sessionButtonInstance = Resources.Load<VisualTreeAsset>("VirtualRobotMenu/SessionButton").CloneTree();
            
            sessionButtonInstance.Q<Label>("SessionName").text = receivedSession.Name;

            var joinedPlayersGroup = sessionButtonInstance.Q<VisualElement>("JoinedPlayersGroup");
            if (receivedSession.Users != null)
                foreach (var user in receivedSession.Users)
                {
                    var playerItem = Resources.Load<VisualTreeAsset>("VirtualRobotMenu/JoinedPlayerItem").CloneTree();
                    
                    playerItem.Q<Label>("PlayerLabel").text = user;
                    joinedPlayersGroup.Add(playerItem);
                }

            sessionButtonInstance.RegisterCallback<ClickEvent>(evt => SetSelectedSession(receivedSession.Name));
            _sessionsGroupScrollView.contentContainer.Add(sessionButtonInstance);
            _sessionButtons.Add(sessionButtonInstance);
        }
    }

    private void OnBackClicked()
    {
        MenuController.Instance.GoBack();
    }

    private void OnJoinClicked()
    {
        if (GlobalClient.Instance.Session != null) return;

        GlobalClient.Instance.JoinSession(_selectionSessionAddress);
        MenuController.Instance.HideMenu();
    }

    private void SetSelectedSession(string selectedSessionAddress)
    {
        _joinButton.SetEnabled(true);

        foreach (var sessionButton in _sessionButtons) sessionButton.RemoveFromClassList("selected-button");

        _selectionSessionAddress = selectedSessionAddress;

        var selectedButton =
            _sessionButtons.Find(button => button.Q<Label>("SessionName").text == selectedSessionAddress);
        selectedButton?.AddToClassList("selected-button");
    }
}