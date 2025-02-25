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
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

#endregion

/// <summary>
/// Manages the UI elements of the playback menu, including button interactions and playback controls.
/// </summary>
public class PlaybackMenuDocument : MonoBehaviour
{
    public UIDocument playbackMenuDocument;
    public PlayableDirector playableDirector;

    private Slider _slider;
    private Toggle _playButton;
    private Label _chapterTitle;
    private Label _sectionTitle;
    private Button _returnButton;
    private Button _completeButton;
    private float _timeSinceLastUpdate;
    private float _playableAssetDuration;
    private bool _isUserInteracting;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the playback menu document and its UI elements.
    /// </summary>
    private void Awake()
    {
        playbackMenuDocument = GetComponent<UIDocument>();
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Sets up button references and event listeners for playback controls.
    /// </summary>
    private void OnEnable()
    {
        _playButton = playbackMenuDocument.rootVisualElement.Q<Toggle>("PlayButton");
        _slider = playbackMenuDocument.rootVisualElement.Q<Slider>("Slider");
        _chapterTitle = playbackMenuDocument.rootVisualElement.Q<Label>("ChapterTitle");
        _sectionTitle = playbackMenuDocument.rootVisualElement.Q<Label>("SectionTitle");
        _returnButton = playbackMenuDocument.rootVisualElement.Q<Button>("ReturnButton");
        _completeButton = playbackMenuDocument.rootVisualElement.Q<Button>("CompleteButton");

        if (_playButton == null || _slider == null || _chapterTitle == null || _sectionTitle == null ||
            _returnButton == null)
        {
            Debug.LogWarning("PlaybackMenuDocument: One or more UI elements are not found.");
            return;
        }

        // Find and assign the PlayableDirector component
        playableDirector = FindObjectOfType<PlayableDirector>();
        if (playableDirector == null)
        {
            Debug.LogWarning("PlaybackMenuDocument: PlayableDirector not found in the scene.");
            return;
        }

        _chapterTitle.text = MenuController.Instance.currentSelectedChapter?.chapterName ?? "Chapter";
        _sectionTitle.text = MenuController.Instance.currentSelectedSection?.title ?? "Section";

        _returnButton.clicked += OnReturnButtonClicked;
        _slider.RegisterValueChangedCallback(OnSliderValueChanged);
        _slider.RegisterCallback<PointerDownEvent>(OnSliderPointerDown);
        _slider.RegisterCallback<PointerUpEvent>(OnSliderPointerUp);
        _playButton.RegisterValueChangedCallback(OnPlayButtonValueChanged);
        playableDirector.played += HandlePlay;
        playableDirector.stopped += HandleStop;
        playableDirector.paused += HandlePause;
        _completeButton.clicked += OnCompleteButtonClicked;

        if (MenuController.Instance.currentSelectedSection)
        {
            PrepareTutorial(MenuController.Instance.currentSelectedSection);
            playableDirector.Play();
            _playButton.value = true;
        }
    }

    private void OnCompleteButtonClicked()
    {
        MenuController.Instance.CompleteSection();
        MenuController.Instance.ChangeMenu(MenuName.Main);
    }

    /// <summary>
    /// Called when the object becomes disabled.
    /// Unregisters event listeners and stops the playable director.
    /// </summary>
    private void OnDisable()
    {
        _returnButton.clicked -= OnReturnButtonClicked;
        _slider.UnregisterValueChangedCallback(OnSliderValueChanged);
        _slider.UnregisterCallback<PointerDownEvent>(OnSliderPointerDown);
        _playButton.UnregisterValueChangedCallback(OnPlayButtonValueChanged);
        playableDirector.stopped -= HandleStop;
        playableDirector.played -= HandlePlay;
        playableDirector.paused -= HandlePause;
        _completeButton.clicked -= OnCompleteButtonClicked;

        if (playableDirector)
            playableDirector.Stop();
    }

    private void Update()
    {
        UpdatePlaybackTime();
    }

    /// <summary>
    /// Because of the way Unity's PlayableDirector works, we need to manually update the playback time.
    /// Otherwise, sound crackling issues occur.
    /// </summary>
    private void UpdatePlaybackTime()
    {
        _slider.value = (float)playableDirector.time / _playableAssetDuration;
        _isUserInteracting = false;
    }

    /// <summary>
    /// Handles the stop event of the playable director.
    /// Resets the play button and timeline to the beginning.
    /// </summary>
    /// <param name="director">The playable director that stopped.</param>
    public void HandleStop(PlayableDirector director)
    {
        _playButton.value = false;
        // Reset the timeline to the beginning
        playableDirector.time = 0;
        playableDirector.Evaluate();
    }

    public void HandlePause(PlayableDirector director)
    {
        _playButton.value = false;
    }

    public void HandlePlay(PlayableDirector director)
    {
        _playButton.value = true;
    }

    /// <summary>
    /// Handles the value change event of the slider.
    /// Updates the playable director's time based on the slider's value.
    /// </summary>
    /// <param name="evt">The change event containing the new value.</param>
    private void OnSliderValueChanged(ChangeEvent<float> evt)
    {
        // Check if the user has interacted with the slider before updating
        if (!_isUserInteracting) return;

        playableDirector.time = playableDirector.duration * evt.newValue;
        playableDirector.Evaluate();
    }

    /// <summary>
    /// Handles the pointer down event on the slider.
    /// Pauses the playable director when the slider is interacted with.
    /// </summary>
    /// <param name="evt">The pointer down event.</param>
    private void OnSliderPointerDown(PointerDownEvent evt)
    {
        _playButton.value = false;
        playableDirector.Pause();
    }

    private void OnSliderPointerUp(PointerUpEvent evt)
    {
        _playButton.value = true;
        playableDirector.Play();
        _isUserInteracting = true;
    }

    /// <summary>
    /// Handles the value change event of the play button.
    /// Plays or pauses the playable director based on the button's state.
    /// </summary>
    /// <param name="evt">The change event containing the new value.</param>
    private void OnPlayButtonValueChanged(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            _slider.pickingMode = PickingMode.Position;
            playableDirector.time = _slider.value * _playableAssetDuration;
            playableDirector.Evaluate();
            playableDirector.Play();
        }
        else
        {
            _slider.pickingMode = PickingMode.Ignore;
            playableDirector.time = _slider.value * _playableAssetDuration;
            playableDirector.Evaluate();
            playableDirector.Pause();
        }
    }

    /// <summary>
    /// Handles the click event of the return button.
    /// Changes the menu to the tutorial and stops the playable director.
    /// </summary>
    private void OnReturnButtonClicked()
    {
        MenuController.Instance.ChangeMenu(MenuName.Main);
        playableDirector.Stop();
    }

    /// <summary>
    /// Prepares the tutorial by setting the playable asset and resetting the slider.
    /// </summary>
    /// <param name="data">The section data containing the timeline asset.</param>
    private void PrepareTutorial(SectionData data)
    {
        playableDirector.playableAsset = data.timelineAsset;
        playableDirector.Stop();

        _slider.value = 0;
        _playableAssetDuration = (float)playableDirector.playableAsset.duration;

        if (_slider.pickingMode == PickingMode.Ignore)
        {
            _slider.pickingMode = PickingMode.Position;
            _playButton.pickingMode = PickingMode.Position;
        }
    }
}