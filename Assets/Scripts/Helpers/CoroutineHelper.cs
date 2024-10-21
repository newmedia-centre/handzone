using System.Collections;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    private static CoroutineHelper _instance;
    private static readonly object _lock = new object();

    public static CoroutineHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var obj = new GameObject("CoroutineHelper");
                        _instance = obj.AddComponent<CoroutineHelper>();
                        DontDestroyOnLoad(obj);
                    }
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