using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginMenuDocument : MonoBehaviour
{
    
    public UIDocument loginMenuDocument;
    
    private Button _loginButton;
    private Button _optionsButton;
    private Button _quitButton;
    private Label _pinLabel;
    private VisualElement _pinDescription;

    private void Awake()
    {
        loginMenuDocument.GetComponent<UIDocument>();
    }

    void OnEnable()
    {
        _loginButton = loginMenuDocument.rootVisualElement.Q<Button>("LoginButton");
        _optionsButton = loginMenuDocument.rootVisualElement.Q<Button>("OptionsButton");
        _quitButton = loginMenuDocument.rootVisualElement.Q<Button>("QuitButton");
        _pinLabel = loginMenuDocument.rootVisualElement.Q<Label>("PinLabel");
        _pinDescription = loginMenuDocument.rootVisualElement.Q<VisualElement>("PinDescription");
        
        _loginButton.clicked += OnLoginButtonClicked;
        _quitButton.clicked += OnQuitButtonClicked;
        _pinLabel.text = "";
        _pinDescription.style.display = DisplayStyle.None;
    }

    private async void OnLoginButtonClicked()
    {
        _pinLabel.text = "Logging in...";
        var pin = await GlobalClient.Instance.GetPin();
        if (string.IsNullOrEmpty(pin))
        {
            Debug.LogWarning("PIN is empty.");
            _pinLabel.text = "Could not get PIN from server.";
            return;
        }
        
        _pinLabel.text = "Pin: " + pin;
        _pinDescription.style.display = DisplayStyle.Flex;

        try
        {
            await GlobalClient.Instance.TryConnectToGlobalServer(pin);
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
            _pinLabel.text = $"An error occurred: {ex.Message}";
        }
    }

    private void OnQuitButtonClicked()
    {
        MenuController.Instance.QuitGame();
    }

    private void OnDisable()
    {
        _loginButton.clicked -= OnLoginButtonClicked;
        _quitButton.clicked -= OnQuitButtonClicked;
        _pinLabel.text = "";
    }
}
