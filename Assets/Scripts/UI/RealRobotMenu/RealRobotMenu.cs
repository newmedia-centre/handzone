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
        GlobalClient.Instance.RequestRealSession();
    }

    /// <summary>
    /// Called before the first frame update.
    /// Sets up the join button listener and event subscriptions.
    /// </summary>
    private void Start()
    {
        _joinButton.onClick.AddListener(() =>
        {
            if (GlobalClient.Instance.Session == null)
            {
                _statusText.text = "No real robot available. Requesting permission...";
                GlobalClient.Instance.RequestRealSession();
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