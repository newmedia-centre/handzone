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
using UnityEngine;
using UnityEngine.UIElements;

#endregion

/// <summary>
/// Manages the UI elements of the login menu, including button interactions and PIN handling.
/// </summary>
public class LoginMenuDocument : MonoBehaviour
{
    public UIDocument loginMenuDocument;

    private Button _loginButton;
    private Button _optionsButton;
    private Button _quitButton;
    private Label _pinLabel;
    private VisualElement _pinDescription;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the UIDocument reference.
    /// </summary>
    private void Awake()
    {
        loginMenuDocument.GetComponent<UIDocument>();
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Sets up button references and event listeners for UI interactions.
    /// </summary>
    private void OnEnable()
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

    /// <summary>
    /// Handles the login button click event.
    /// Retrieves the PIN and attempts to connect to the global server.
    /// </summary>
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

    /// <summary>
    /// Handles the quit button click event.
    /// Exits the game.
    /// </summary>
    private void OnQuitButtonClicked()
    {
        MenuController.Instance.QuitGame();
    }

    /// <summary>
    /// Called when the object becomes disabled.
    /// Unsubscribes from button click events.
    /// </summary>
    private void OnDisable()
    {
        _loginButton.clicked -= OnLoginButtonClicked;
        _quitButton.clicked -= OnQuitButtonClicked;
        _pinLabel.text = "";
    }
}