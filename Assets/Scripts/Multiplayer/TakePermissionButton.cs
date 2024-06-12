using UnityEngine;
using UnityEngine.UI;

public class TakePermissionButton : MonoBehaviour
{
    private Button _button;
    
    // Start is called before the first frame update
    void Start()
    {
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
        if (SessionClient.Instance)
            SessionClient.Instance.TakeControlPermission();
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(RequestPermission);
    }
}
