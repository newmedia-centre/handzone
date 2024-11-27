using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class RealRobotMenuDocument : MonoBehaviour
{
    public UIDocument realRobotDocument;

    private Label _statusLabel;
    private Button _joinButton;
    private Button _backButton;
    private Button _returnButton;

    private void Awake()
    {
        realRobotDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _statusLabel = realRobotDocument.rootVisualElement.Q<Label>("StatusLabel");
        _joinButton = realRobotDocument.rootVisualElement.Q<Button>("JoinButton");
        _backButton = realRobotDocument.rootVisualElement.Q<Button>("BackButton");
        _returnButton = realRobotDocument.rootVisualElement.Q<Button>("ReturnButton");

        _joinButton.clicked += OnJoinClicked;
        _backButton.clicked += OnBackClicked;
        _returnButton.clicked += OnReturnButtonClicked;
    }

    private void OnDisable()
    {
        _joinButton.clicked -= OnJoinClicked;
        _backButton.clicked -= OnBackClicked;
        _returnButton.clicked -= OnReturnButtonClicked;
    }

    private void OnReturnButtonClicked()
    {
        MenuController.Instance.ChangeMenu(MenuName.Main);
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

    private async void OnJoinClicked()
    {
        _statusLabel.text = "No real robot available. Requesting permission...";
        var success = await GlobalClient.Instance.RequestRealSession();
        
        if (!success) 
        {
            _statusLabel.text = "Failed to request real robot session.";
        }
        // If the request was successful, load the session scene
        else
        {
            _statusLabel.text = "Joining...";
            StartCoroutine(Utility.LoadSceneCoroutine("Scenes/Session"));
            MenuController.Instance.HideMenu();
        }
    }
}
