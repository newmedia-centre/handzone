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

#endregion

/// <summary>
/// The Timer class provides functionality to manage a countdown timer.
/// It allows starting, stopping, resetting, and setting the duration of the timer.
/// The timer updates every frame and can be queried for its remaining time and duration.
/// </summary>
public class Timer : MonoBehaviour
{
    private float _timeRemaining = 10;
    private bool _timerStarted;
    private float _timerDuration;

    /// <summary>
    /// Starts the timer with the previously set duration.
    /// </summary>
    public void StartTimer()
    {
        _timerStarted = true;
        _timeRemaining = _timerDuration;
    }

    /// <summary>
    /// Stops the timer.
    /// </summary>
    public void StopTimer()
    {
        _timerStarted = false;
    }

    /// <summary>
    /// Resets the timer to the previously set duration.
    /// </summary>
    public void Reset()
    {
        _timeRemaining = _timerDuration;
    }

    /// <summary>
    /// Sets the duration of the timer.
    /// </summary>
    /// <param name="duration">The duration in seconds to set for the timer.</param>
    public void SetTimerDuration(float duration)
    {
        _timerDuration = duration;
    }

    /// <summary>
    /// Updates the timer every frame.
    /// If the timer is started and time remains, it decrements the remaining time.
    /// If the time runs out, it stops the timer.
    /// </summary>
    private void Update()
    {
        if (_timerStarted)
        {
            if (_timeRemaining > 0)
                _timeRemaining -= Time.deltaTime;
            else
                StopTimer();
        }
    }

    /// <summary>
    /// Checks if the timer is currently running.
    /// </summary>
    /// <returns>True if the timer is started; otherwise, false.</returns>
    public bool Started()
    {
        return _timerStarted;
    }

    /// <summary>
    /// Gets the remaining time of the timer.
    /// </summary>
    /// <returns>The remaining time in seconds.</returns>
    public float TimeRemaining()
    {
        return _timeRemaining;
    }

    /// <summary>
    /// Gets the total duration of the timer.
    /// </summary>
    /// <returns>The duration in seconds.</returns>
    public float TimeDuration()
    {
        return _timerDuration;
    }
}