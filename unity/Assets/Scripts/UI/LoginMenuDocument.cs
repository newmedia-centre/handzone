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