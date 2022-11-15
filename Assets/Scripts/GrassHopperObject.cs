using Unity.VisualScripting;
using UnityEngine;

public class GrassHopperObject : MonoBehaviour
{
    public bool sendPosition = true;
    public bool sendRotation = true;
    public float updateDuration = 3;

    private const int SCALE = 1000;
    private Timer _timer;
    private bool _shouldUpdate = true;

    private void Awake()
    {
        _timer = this.AddComponent<Timer>();
        _timer.SetTimerDuration(updateDuration);
    }

    private void Update()
    {
        if (transform.hasChanged && !_timer.Started() && _shouldUpdate)
        {
            Debug.Log(_shouldUpdate);
            if (!sendPosition && !sendRotation)
                return;
                
            if (sendPosition)
            {
                UnityInGrasshopper.Instance.SendPosition(transform.position * SCALE, name);
            }
            if (sendRotation)
            {
                UnityInGrasshopper.Instance.SendRotationQuaternion(transform.rotation, name);
            }

            transform.hasChanged = false;
            _timer.StartTimer();
        }
    }

    public void ShouldUpdate(bool value)
    {
        _shouldUpdate = value;
    }
}
