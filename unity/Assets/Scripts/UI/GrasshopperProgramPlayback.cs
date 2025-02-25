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