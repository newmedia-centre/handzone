using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RealRobotMenuDocument : MonoBehaviour
{
    public UIDocument realRobotDocument;

    private Label _statusLabel;
    private Button _joinButton;
    private Button _backButton;
    
    private void Awake()
    {
        realRobotDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _statusLabel = realRobotDocument.rootVisualElement.Q<Label>("StatusLabel");
        _joinButton = realRobotDocument.rootVisualElement.Q<Button>("JoinButton");
        _backButton = realRobotDocument.rootVisualElement.Q<Button>("BackButton");
        
        _joinButton.clicked += OnJoinClicked;
        _backButton.clicked += OnBackClicked;
    }

    private void Start()
    {
        GlobalClient.Instance.OnSessionJoin += DisplayAvailability;
        GlobalClient.Instance.OnSessionJoinFailed += DisplayError;
    }

    private void DisplayError(string err)
    {
        _statusLabel.text = err;
        Debug.LogWarning(err);
    }

    private void DisplayAvailability(string obj)
    {
        _statusLabel.text = "Real robot available, press Join to connect.";
    }

    private void OnBackClicked()
    {
        MenuController.Instance.GoBack();
    }

    private void OnJoinClicked()
    {
        if (GlobalClient.Instance.Session == null)
        {
            _statusLabel.text = "No real robot available. Requesting permission...";
            GlobalClient.Instance.RequestRealSession();
            return;
        }
        
        _statusLabel.text = "Joining...";
        StartCoroutine(Utility.LoadSceneCoroutine("Scenes/Session"));
        MenuController.Instance.HideMenu();
    }
}
