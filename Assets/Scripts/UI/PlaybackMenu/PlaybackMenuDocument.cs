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
        _playButton.RegisterValueChangedCallback(OnPlayButtonValueChanged);
        playableDirector.stopped += HandleStop;
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
    /// Updates the slider value based on the current time of the playable director.
    /// </summary>
    private void Update()
    {
        _slider.value = (float)(playableDirector.time / playableDirector.duration);
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
        _completeButton.clicked -= OnCompleteButtonClicked;

        if (playableDirector)
            playableDirector.Stop();
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

    /// <summary>
    /// Handles the value change event of the slider.
    /// Updates the playable director's time based on the slider's value.
    /// </summary>
    /// <param name="evt">The change event containing the new value.</param>
    private void OnSliderValueChanged(ChangeEvent<float> evt)
    {
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
            playableDirector.time = _slider.value * playableDirector.duration;
            playableDirector.Evaluate();
            playableDirector.Play();
        }
        else
        {
            _slider.pickingMode = PickingMode.Ignore;
            playableDirector.time = _slider.value * playableDirector.duration;
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
        playableDirector.time = 0;

        _slider.value = 0;

        if (_slider.pickingMode == PickingMode.Ignore)
        {
            _slider.pickingMode = PickingMode.Position;
            _playButton.pickingMode = PickingMode.Position;
        }
    }
}