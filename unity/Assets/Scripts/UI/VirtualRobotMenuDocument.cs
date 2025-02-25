/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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