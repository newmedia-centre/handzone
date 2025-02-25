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