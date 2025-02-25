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

public class PlaybackPanel : MonoBehaviour
{
    public Button playButton;
    public Button pauseButton;
    public SliderPanel sliderPanel;

    private int _programDuration;

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Subscribes to events for time and program duration updates.
    /// </summary>
    private void OnEnable()
    {
        RobotActions.OnTimeUpdated += UpdatePlaybackTime;
        RobotActions.OnProgramDurationUpdated += UpdateProgramDuration;
    }

    /// <summary>
    /// Updates the playback time displayed in the slider panel.
    /// </summary>
    /// <param name="time">The current playback time.</param>
    private void UpdatePlaybackTime(float time)
    {
        sliderPanel.value.text = (int)time + "-" + _programDuration;
        sliderPanel.slider.value = time;
    }

    /// <summary>
    /// Updates the program duration and sets the maximum value of the slider.
    /// </summary>
    /// <param name="duration">The new program duration.</param>
    private void UpdateProgramDuration(int duration)
    {
        _programDuration = duration;
        sliderPanel.slider.maxValue = _programDuration;
    }
}