using System;
using UnityEngine;
using UnityEngine.UI;

public class GrasshopperProgramPlayback : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _pauseButton;
    
    private void OnEnable()
    {
        // Subscribe to the PlayProgram and PauseProgram events
        _playButton.onClick.AddListener(PlayProgram);
        _pauseButton.onClick.AddListener(PauseProgram);
        
        // Set the initial state of the buttons
        _playButton.gameObject.SetActive(true);
        _pauseButton.gameObject.SetActive(false);
    }

    private void PlayProgram()
    {
        if(SessionClient.Instance == null)
        {
            Debug.LogError("Session is not created");
            return;
        }
        
        SessionClient.Instance.PlayProgram();
        _pauseButton.gameObject.SetActive(true);
        _playButton.gameObject.SetActive(false);
    }

    private void PauseProgram()
    {
        if(SessionClient.Instance == null)
        {
            Debug.LogError("Session is not created");
            return;
        }
        
        SessionClient.Instance.PauseProgram();
        _pauseButton.gameObject.SetActive(false);
        _playButton.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        // Unsubscribe from the PlayProgram and PauseProgram events
        _playButton.onClick.RemoveListener(PlayProgram);
        _pauseButton.onClick.RemoveListener(PauseProgram);
    }
}
