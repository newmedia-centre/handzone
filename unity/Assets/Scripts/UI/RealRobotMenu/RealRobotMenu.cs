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

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion

/// <summary>
/// Manages the UI elements of the real robot menu, including session requests and button interactions.
/// </summary>
public class RealRobotMenu : MonoBehaviour
{
    private Button _joinButton;
    private TMP_Text _statusText;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the button and status text references.
    /// </summary>
    private void Awake()
    {
        transform.Find("SessionPanel/Buttons/JoinButton").TryGetComponent(out _joinButton);
        transform.Find("SessionPanel/SessionsGroup/Status").TryGetComponent(out _statusText);
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Resets the status text.
    /// </summary>
    private void OnEnable()
    {
        _statusText.text = "";
    }

    /// <summary>
    /// Called before the first frame update.
    /// Sets up the join button listener and event subscriptions.
    /// </summary>
    private void Start()
    {
        _joinButton.onClick.AddListener(async () =>
        {
            if (GlobalClient.Instance.Session == null)
            {
                _statusText.text = "No real robot available. Requesting permission...";
                await GlobalClient.Instance.RequestRealSession();
                return;
            }

            _statusText.text = "Joining...";
            StartCoroutine(LoadSceneCoroutine("Scenes/Session"));
        });

        GlobalClient.Instance.OnSessionJoin += DisplayAvailability;
        GlobalClient.Instance.OnSessionJoinFailed += DisplayError;
    }

    /// <summary>
    /// Displays a message indicating that a real robot is available.
    /// </summary>
    /// <param name="robotName">The name of the available robot.</param>
    private void DisplayAvailability(string robotName)
    {
        _statusText.text = "Real robot available, press join to connect.";
    }

    /// <summary>
    /// Displays an error message when session joining fails.
    /// </summary>
    /// <param name="error">The error message.</param>
    private void DisplayError(string error)
    {
        _statusText.text = error;
        Debug.LogWarning(error);
    }

    /// <summary>
    /// Called when the object is destroyed.
    /// Unsubscribes from events and removes button listeners.
    /// </summary>
    private void OnDestroy()
    {
        _joinButton.onClick.RemoveListener(() => { _statusText.text = ""; });

        GlobalClient.Instance.OnSessionJoin -= DisplayAvailability;
        GlobalClient.Instance.OnSessionJoinFailed -= DisplayError;
    }

    /// <summary>
    /// Loads a scene asynchronously.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <returns>An enumerator for the coroutine.</returns>
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Start loading the scene
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // Wait until the scene is fully loaded
        while (asyncLoad is { isDone: false }) yield return null;
    }
}