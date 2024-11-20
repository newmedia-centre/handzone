using System;
using System.Collections.Generic;
using Schema.Socket.Index;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class VirtualRobotMenuDocument : MonoBehaviour
{
    public UIDocument virtualRobotDocument;

    private ScrollView _sessionsGroupScrollView;
    private Button _joinButton;
    private Button _createButton;
    private Button _backButton;
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
        _backButton = virtualRobotDocument.rootVisualElement.Q<Button>("BackButton");
        
        _joinButton.SetEnabled(false);
        _joinButton.focusable = false;

        _joinButton.clicked += OnJoinClicked;
        _createButton.clicked += OnCreateClicked;
        _backButton.clicked += OnBackClicked;

        if (GlobalClient.Instance?.Sessions != null)
        {
            UpdateMenu(GlobalClient.Instance.Sessions);
        }
    }

    private void Start() 
    {
        GlobalClient.Instance.OnSessionsReceived += UpdateMenu;


        if (GlobalClient.Instance?.Sessions != null)
        {
            UpdateMenu(GlobalClient.Instance.Sessions);
        }
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

            var sessionButtonInstance = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/VirtualRobotMenu/SessionButton.uxml").CloneTree();
            sessionButtonInstance.Q<Label>("SessionName").text = receivedSession.Name;

            var joinedPlayersGroup = sessionButtonInstance.Q<VisualElement>("JoinedPlayersGroup");
            if (receivedSession.Users != null)
            {
                foreach (var user in receivedSession.Users)
                {
                    var playerItem = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/VirtualRobotMenu/JoinedPlayerItem.uxml").CloneTree();
                    playerItem.Q<Label>("PlayerLabel").text = user;
                    joinedPlayersGroup.Add(playerItem);
                }
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

        foreach (var sessionButton in _sessionButtons)
        {
            sessionButton.RemoveFromClassList("selected-button");
        }

        _selectionSessionAddress = selectedSessionAddress;

        var selectedButton = _sessionButtons.Find(button => button.Q<Label>("SessionName").text == selectedSessionAddress);
        selectedButton?.AddToClassList("selected-button");
    }
}