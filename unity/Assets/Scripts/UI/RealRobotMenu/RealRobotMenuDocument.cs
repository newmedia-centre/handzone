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

using UnityEngine;
using UnityEngine.UIElements;

#endregion

/// <summary>
/// Manages the UI elements of the real robot menu document, including button interactions and session management.
/// </summary>
public class RealRobotMenuDocument : MonoBehaviour
{
    public UIDocument realRobotDocument;

    private Label _statusLabel;
    private Button _joinButton;
    private Button _backButton;
    private Button _returnButton;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the UIDocument reference.
    /// </summary>
    private void Awake()
    {
        realRobotDocument = GetComponent<UIDocument>();
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Sets up button references and event listeners for UI interactions.
    /// </summary>
    private void OnEnable()
    {
        _statusLabel = realRobotDocument.rootVisualElement.Q<Label>("StatusLabel");
        _joinButton = realRobotDocument.rootVisualElement.Q<Button>("JoinButton");
        _backButton = realRobotDocument.rootVisualElement.Q<Button>("BackButton");
        _returnButton = realRobotDocument.rootVisualElement.Q<Button>("ReturnButton");

        _joinButton.clicked += OnJoinClicked;
        _backButton.clicked += OnBackClicked;
        _returnButton.clicked += OnBackClicked;
    }

    /// <summary>
    /// Called when the object becomes disabled.
    /// Unsubscribes from button click events.
    /// </summary>
    private void OnDisable()
    {
        _joinButton.clicked -= OnJoinClicked;
        _backButton.clicked -= OnBackClicked;
        _returnButton.clicked -= OnBackClicked;
    }

    /// <summary>
    /// Called before the first frame update.
    /// Subscribes to session join events.
    /// </summary>
    private void Start()
    {
        GlobalClient.Instance.OnSessionJoin += DisplayAvailability;
        GlobalClient.Instance.OnSessionJoinFailed += DisplayError;
    }

    /// <summary>
    /// Displays an error message when session joining fails.
    /// </summary>
    /// <param name="err">The error message.</param>
    private void DisplayError(string err)
    {
        _statusLabel.text = err;
        Debug.LogWarning(err);
    }

    /// <summary>
    /// Displays a message indicating that a real robot is available.
    /// </summary>
    /// <param name="obj">The object representing the availability of the robot.</param>
    private void DisplayAvailability(string obj)
    {
        _statusLabel.text = "Real robot available, press Join to connect.";
    }

    /// <summary>
    /// Called when the back button is clicked.
    /// Navigates back in the menu.
    /// </summary>
    private void OnBackClicked()
    {
        MenuController.Instance.GoBack();
    }

    /// <summary>
    /// Called when the join button is clicked.
    /// Requests a real session and handles the response.
    /// </summary>
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