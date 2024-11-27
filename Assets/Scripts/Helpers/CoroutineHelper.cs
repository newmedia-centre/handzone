using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    private static CoroutineHelper _instance;

    public static CoroutineHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CoroutineHelper>();
                if (_instance == null)
                {
                    var go = new GameObject("CoroutineHelper");
                    _instance = go.AddComponent<CoroutineHelper>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}