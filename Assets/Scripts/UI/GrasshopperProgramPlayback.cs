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

using UnityEngine;
using UnityEngine.UI;

#endregion

/// <summary>
/// Manages the playback of the Grasshopper program, including play and pause functionality.
/// </summary>
public class GrasshopperProgramPlayback : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _pauseButton;

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Subscribes to button click events and sets the initial button states.
    /// </summary>
    private void OnEnable()
    {
        // Subscribe to the PlayProgram and PauseProgram events
        _playButton.onClick.AddListener(PlayProgram);
        _pauseButton.onClick.AddListener(PauseProgram);

        // Set the initial state of the buttons
        _playButton.gameObject.SetActive(true);
        _pauseButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Plays the Grasshopper program if a session is available.
    /// </summary>
    private void PlayProgram()
    {
        if (SessionClient.Instance == null)
        {
            Debug.LogError("Session is not created");
            return;
        }

        SessionClient.Instance.PlayProgram();
        _pauseButton.gameObject.SetActive(true);
        _playButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Pauses the Grasshopper program if a session is available.
    /// </summary>
    private void PauseProgram()
    {
        if (SessionClient.Instance == null)
        {
            Debug.LogError("Session is not created");
            return;
        }

        SessionClient.Instance.PauseProgram();
        _pauseButton.gameObject.SetActive(false);
        _playButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called when the object is disabled.
    /// Unsubscribes from button click events.
    /// </summary>
    private void OnDisable()
    {
        // Unsubscribe from the PlayProgram and PauseProgram events
        _playButton.onClick.RemoveListener(PlayProgram);
        _pauseButton.onClick.RemoveListener(PauseProgram);
    }
}