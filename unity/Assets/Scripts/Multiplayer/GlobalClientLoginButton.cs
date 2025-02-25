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