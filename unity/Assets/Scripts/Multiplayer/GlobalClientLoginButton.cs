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
using PimDeWitte.UnityMainThreadDispatcher;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

/// <summary>
/// The GlobalClientLoginButton class manages the login process for the global client.
/// It handles user interactions with the login button, manages the display of
/// connection status, and communicates with the MainMenuController to change menus
/// upon successful connection. The class also manages the display of error messages
/// and logs during the connection process.
/// </summary>
[RequireComponent(typeof(Button))]
public class GlobalClientLoginButton : MonoBehaviour
{
    public MainMenuController mainMenuController;
    public Button loginButton;
    public TMP_Text pinText;
    public GameObject logPanel;

    private MenuControllerOption _menuControllerOption;
    private TMP_Text _logText;
    public UnityEvent OnConnectionSuccess;

    /// <summary>
    /// Initializes the button and sets up event listeners for connection events.
    /// </summary>
    private void Start()
    {
        if (mainMenuController == null)
            mainMenuController = FindObjectOfType<MainMenuController>();

        TryGetComponent(out _menuControllerOption);

        if (logPanel == null)
            logPanel = GameObject.Find("LogPanel");

        _logText = logPanel.GetComponentInChildren<TMP_Text>();
        logPanel.SetActive(false);
        _logText.text = "";

        if (loginButton == null)
            loginButton = GetComponent<Button>();
        loginButton.onClick.AddListener(GlobalClientLogin);

        GlobalClient.Instance.OnConnecting += () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (loginButton == null || pinText == null)
                    return;

                loginButton.interactable = false;
                loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connecting...";
            });
        };

        GlobalClient.Instance.OnConnected += () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (loginButton == null || pinText == null)
                    return;

                loginButton.interactable = true;
                pinText.text = "";
                loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connected";
                OnConnectionSuccess.Invoke();

                mainMenuController.ChangeMenu(_menuControllerOption);
            });
        };

        GlobalClient.Instance.OnDisconnected += () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (loginButton == null || pinText == null)
                    return;

                loginButton.interactable = true;
                loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Login";
            });
        };

        GlobalClient.Instance.OnError += (error) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (loginButton == null || pinText == null)
                    return;

                loginButton.interactable = true;
                loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Login";
                Debug.LogError($"An error occurred: {error}");
                _logText.text = $"An error occurred: {error}";
            });
        };
    }

    /// <summary>
    /// Button click event to login to the global server. It checks if the PIN is empty
    /// and attempts to connect to the server. It also handles error logging and
    /// displays messages in the log panel.
    /// </summary>
    private async void GlobalClientLogin()
    {
        var pin = await GlobalClient.Instance.GetPin();
        if (string.IsNullOrEmpty(pin))
        {
            Debug.LogWarning("PIN is empty.");
            logPanel.SetActive(true);
            _logText.text = "Could not get PIN from server.";
            return;
        }

        pinText.text = pin;
        Debug.Log(pin);

        if (logPanel.activeSelf)
            logPanel.SetActive(false);

        try
        {
            await GlobalClient.Instance.TryConnectToGlobalServer(pin);
            // mainMenuController.ChangeMenu(_menuControllerOption);
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
            logPanel.SetActive(true);
            _logText.text = $"An error occurred: {ex.Message}";
        }
    }
}