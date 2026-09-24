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
using UnityEngine.Video;
using UnityEngine.UIElements;
using VNCScreen;

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
    private bool _isUpdatingSliderFromPlayback;
    private VideoPlayer _boundVideoPlayer;
    private Renderer _boundVideoRenderer;
    private VNCScreen.VNCScreen _boundVncScreen;
    private Material _cachedScreenMaterial;
    private bool _cachedVncScreenEnabled;

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
        MenuController.Instance.ChangeMenu(MenuName.Tutorial);
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
        _slider.UnregisterCallback<PointerUpEvent>(OnSliderPointerUp);
        _playButton.UnregisterValueChangedCallback(OnPlayButtonValueChanged);
        _completeButton.clicked -= OnCompleteButtonClicked;

        if(playableDirector == null)
        return;

        playableDirector.stopped -= HandleStop;
        playableDirector.played -= HandlePlay;
        playableDirector.paused -= HandlePause;
        RestoreVideoScreenState();
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
        if (_slider == null || playableDirector == null || playableDirector.playableAsset == null || _isUserInteracting ||
            _playableAssetDuration <= 0f)
            return;

        _isUpdatingSliderFromPlayback = true;
        _slider.SetValueWithoutNotify((float)playableDirector.time / _playableAssetDuration);
        _isUpdatingSliderFromPlayback = false;
    }

    /// <summary>
    /// Handles the stop event of the playable director.
    /// Resets the play button and timeline to the beginning.
    /// </summary>
    /// <param name="director">The playable director that stopped.</param>
    public void HandleStop(PlayableDirector director)
    {
        _playButton.SetValueWithoutNotify(false);
        // Reset the timeline to the beginning
        playableDirector.time = 0;
        playableDirector.Evaluate();
    }

    public void HandlePause(PlayableDirector director)
    {
        _playButton.SetValueWithoutNotify(false);
        playableDirector.Evaluate();
    }

    public void HandlePlay(PlayableDirector director)
    {
        _playButton.SetValueWithoutNotify(true);
    }

    /// <summary>
    /// Handles the value change event of the slider.
    /// Updates the playable director's time based on the slider's value.
    /// </summary>
    /// <param name="evt">The change event containing the new value.</param>
    private void OnSliderValueChanged(ChangeEvent<float> evt)
    {
        if (_isUpdatingSliderFromPlayback || playableDirector == null || _playableAssetDuration <= 0f)
            return;

        playableDirector.time = evt.newValue * _playableAssetDuration;
        playableDirector.Evaluate();
    }

    /// <summary>
    /// Handles the pointer down event on the slider.
    /// Pauses the playable director when the slider is interacted with.
    /// </summary>
    /// <param name="evt">The pointer down event.</param>
    private void OnSliderPointerDown(PointerDownEvent evt)
    {
        _isUserInteracting = true;
        playableDirector.Pause();
        playableDirector.Evaluate();
    }

    private void OnSliderPointerUp(PointerUpEvent evt)
    {
        _isUserInteracting = false;

        playableDirector.time = _slider.value * _playableAssetDuration;
        playableDirector.Evaluate();
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
            playableDirector.time = _slider.value * _playableAssetDuration;
            playableDirector.Evaluate();
            playableDirector.Play();
        }
        else
        {
            playableDirector.Pause();
            playableDirector.time = _slider.value * _playableAssetDuration;
            playableDirector.Evaluate();
        }
    }

    /// <summary>
    /// Handles the click event of the return button.
    /// Changes the menu to the tutorial and stops the playable director.
    /// </summary>
    private void OnReturnButtonClicked()
    {
        MenuController.Instance.ChangeMenu(MenuName.Tutorial);
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
        BindTutorialVideoScreen();

        _slider.SetValueWithoutNotify(0);
        _playableAssetDuration = (float)playableDirector.playableAsset.duration;
    }

    private void BindTutorialVideoScreen()
    {
        RestoreVideoScreenState();

        _boundVideoPlayer = FindBoundVideoPlayer();
        if (_boundVideoPlayer == null)
            return;

        _boundVideoRenderer = _boundVideoPlayer.GetComponent<Renderer>();
        _boundVncScreen = _boundVideoPlayer.GetComponent<VNCScreen.VNCScreen>();

        if (_boundVideoRenderer != null)
        {
            _cachedScreenMaterial = _boundVideoRenderer.sharedMaterial;

            var screenMaterial = _boundVideoRenderer.material;
            if (_boundVideoPlayer.targetTexture != null)
            {
                if (screenMaterial.HasProperty("_BaseMap"))
                    screenMaterial.SetTexture("_BaseMap", _boundVideoPlayer.targetTexture);

                if (screenMaterial.HasProperty("_MainTex"))
                    screenMaterial.SetTexture("_MainTex", _boundVideoPlayer.targetTexture);
            }

            if (screenMaterial.HasProperty("_BaseColor"))
                screenMaterial.SetColor("_BaseColor", Color.white);

            if (screenMaterial.HasProperty("_Color"))
                screenMaterial.color = Color.white;
        }

        if (_boundVncScreen != null)
        {
            _cachedVncScreenEnabled = _boundVncScreen.enabled;
            _boundVncScreen.enabled = false;
        }
    }

    private void RestoreVideoScreenState()
    {
        if (_boundVideoRenderer != null && _cachedScreenMaterial != null)
            _boundVideoRenderer.sharedMaterial = _cachedScreenMaterial;

        if (_boundVncScreen != null)
            _boundVncScreen.enabled = _cachedVncScreenEnabled;

        _boundVideoPlayer = null;
        _boundVideoRenderer = null;
        _boundVncScreen = null;
        _cachedScreenMaterial = null;
        _cachedVncScreenEnabled = false;
    }

    private VideoPlayer FindBoundVideoPlayer()
    {
        if (playableDirector?.playableAsset == null)
            return null;

        foreach (var output in playableDirector.playableAsset.outputs)
        {
            if (output.outputTargetType != typeof(VideoPlayer))
                continue;

            if (playableDirector.GetGenericBinding(output.sourceObject) is VideoPlayer videoPlayer)
                return videoPlayer;
        }

        return null;
    }
}
