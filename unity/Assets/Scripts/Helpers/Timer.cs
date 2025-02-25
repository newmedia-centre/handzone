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