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
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

#endregion

/// <summary>
/// Manages the tutorial menu, including playback control and tutorial data handling.
/// </summary>
public class TutorialMenuController : MonoBehaviour
{
    [Header("UI Elements")] public Toggle playToggle;
    public Slider videoSlider;

    [Header("Director")] public PlayableDirector director;

    [Header("Images")] public GameObject playImage;
    public GameObject playBackground;
    public GameObject pauseImage;

    private bool buttonState = false;

    [Header("Tutorial list references")] public GameObject viewportContent;
    public GameObject listElementPrefab;

    private bool _isSliderCodeUpdate = false;

    public static bool CustomDirectorChange = false;

    private void Awake()
    {
        videoSlider.onValueChanged.AddListener(SliderChange);

        director.stopped += TimelineEnd;
    }

    /// <summary>
    /// Called when the tutorial menu is entered, initializing the tutorial data.
    /// </summary>
    /// <param name="tutorialData">The array of tutorial data to be displayed.</param>
    public void Enter(TutorialData[] tutorialData)
    {
        FillTutorialScroller(tutorialData);
    }

    /// <summary>
    /// Called when the tutorial menu is exited, cleaning up resources.
    /// </summary>
    public void Exit()
    {
        ChangeButton(false);

        if (director.state == PlayState.Playing) director.Stop();

        for (var i = viewportContent.transform.childCount; i > 0; i--)
            Destroy(viewportContent.transform.GetChild(i - 1).GameObject());

        director.playableAsset = null;
        videoSlider.interactable = false;
        playToggle.interactable = false;
    }

    /// <summary>
    /// Fills the tutorial scroller with the provided tutorial data.
    /// </summary>
    /// <param name="tutorialData">The array of tutorial data to fill the scroller with.</param>
    private void FillTutorialScroller(TutorialData[] tutorialData)
    {
        foreach (var _data in tutorialData)
        {
            var _element = Instantiate(listElementPrefab, viewportContent.transform);
            var _text = _element.GetComponentInChildren<TMP_Text>();
            _text.text = _data.name;

            var _button = _element.GetComponentInChildren<Button>();
            _button.onClick.AddListener(() => PrepareTutorial(_data));
        }
    }

    /// <summary>
    /// Prepares the selected tutorial for playback.
    /// </summary>
    /// <param name="data">The tutorial data to prepare.</param>
    private void PrepareTutorial(TutorialData data)
    {
        director.playableAsset = data.timeline;
        director.time = 0;

        _isSliderCodeUpdate = true;
        videoSlider.value = 0;
        _isSliderCodeUpdate = false;

        if (videoSlider.interactable == false)
        {
            videoSlider.interactable = true;
            playToggle.interactable = true;
        }

        ChangeButton(false);
    }

    /// <summary>
    /// Handles the slider change event to update the playback time.
    /// </summary>
    /// <param name="value">The new value of the slider.</param>
    public void SliderChange(float value)
    {
        if (!_isSliderCodeUpdate)
        {
            director.time = director.duration * value;

            CustomDirectorChange = true;
            director.Evaluate();
            CustomDirectorChange = false;
        }
    }

    /// <summary>
    /// Called when the timeline playback ends.
    /// </summary>
    /// <param name="playableDirector">The playable director that stopped.</param>
    public void TimelineEnd(PlayableDirector playableDirector)
    {
        ChangeButton(false);
    }

    /// <summary>
    /// Changes the play/pause button state.
    /// </summary>
    /// <param name="play">True to play, false to pause.</param>
    public void ChangeButton(bool play)
    {
        playImage.SetActive(!play);
        playBackground.SetActive(play);
        pauseImage.SetActive(play);

        buttonState = play;
    }

    public void OnButtonClick()
    {
        if (buttonState)
        {
            ChangeButton(false);
            director.Pause();
        }
        else
        {
            ChangeButton(true);
            director.Play();
        }
    }

    public void FixedUpdate()
    {
        if (director.state == PlayState.Playing)
        {
            _isSliderCodeUpdate = true;
            videoSlider.value = (float)(director.time / director.duration);
            _isSliderCodeUpdate = false;
        }
    }
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TutorialMenuData", order = 1)]
public class TutorialMenuData : ScriptableObject
{
    public TutorialData[] data;
}

[Serializable]
public struct TutorialData
{
    public string name;
    public TimelineAsset timeline;
}