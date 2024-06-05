using System;
using UnityEngine;
using UnityEngine.UI;

public class PermissionRequestButton : MonoBehaviour
{
    private Button _button;
    
    // Start is called before the first frame update
    void Start()
    {
        if(SessionClient.Instance == null)
        {
            Debug.LogWarning("SessionClient instance is null. Make sure to have a SessionClient instance in the scene.");
            return;
        }

        if (TryGetComponent(out _button)) 
        {
            _button.onClick.AddListener(RequestPermission);
        }
        else
        {
            Debug.LogWarning("Button component not found.");
        }
    }

    private void RequestPermission()
    {
        SessionClient.Instance.RequestPermission();
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(RequestPermission);
    }
}
