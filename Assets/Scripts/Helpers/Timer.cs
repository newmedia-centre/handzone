using UnityEngine;

public class Timer : MonoBehaviour
{
    private float _timeRemaining = 10;
    private bool _timerStarted;
    private float _timerDuration;

    public void StartTimer()
    {
        _timerStarted = true;
        _timeRemaining = _timerDuration;
    }

    public void StopTimer()
    {
        _timerStarted = false;
    }

    public void Reset()
    {
        _timeRemaining = _timerDuration;
    }

    public void SetTimerDuration(float duration)
    {
        _timerDuration = duration;
    }

    void Update()
    {
        if (_timerStarted)
        {
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
            }
            else
            {
                StopTimer();
            }
        }
    }

    public bool Started()
    {
        return _timerStarted;
    }

    public float TimeRemaining()
    {
        return _timeRemaining;
    }

    public float TimeDuration()
    {
        return _timerDuration;
    }
}