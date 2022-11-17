using System;
using UnityEngine;
using UnityEngine.UI;

public class PlaybackPanel : MonoBehaviour
{
    public Button playButton;
    public Button pauseButton;
    public SliderPanel sliderPanel;

    private int _programDuration;

    private void OnEnable()
    {
        RobotActions.OnTimeUpdated += UpdatePlaybackTime;
        RobotActions.OnProgramDurationUpdated += UpdateProgramDuration;
    }

    void UpdatePlaybackTime(float time)
    {
        sliderPanel.value.text = (int)time + "-" + _programDuration;
        sliderPanel.slider.value = time;
    }

    void UpdateProgramDuration(int duration)
    {
        _programDuration = duration;
        sliderPanel.slider.maxValue = _programDuration;
    }
}
