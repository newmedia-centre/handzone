using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RealRobotMenu : MonoBehaviour
{
    private Button _joinButton;
    private TMP_Text _statusText;

    private void Awake()
    {
        transform.Find("SessionPanel/Buttons/JoinButton").TryGetComponent(out _joinButton);
        transform.Find("SessionPanel/SessionsGroup/Status").TryGetComponent(out _statusText);
    }

    private void OnEnable()
    {
        _statusText.text = "";
        GlobalClient.Instance.RequestRealSession();
    }

    // Start is called before the first frame update
    void Start()
    {
        _joinButton.onClick.AddListener(() =>
        {
            if (GlobalClient.Instance.Session == null)
            {
                _statusText.text = "No real robot available. Requesting permission...";
                GlobalClient.Instance.RequestRealSession();
                return;
            }
            
            _statusText.text = "Joining...";
            StartCoroutine(LoadSceneCoroutine("Scenes/UR Robot Scene"));
        });

        GlobalClient.Instance.OnSessionJoin += DisplayAvailability;
        GlobalClient.Instance.OnSessionJoinFailed += DisplayError;
    }

    private void DisplayAvailability(string robotName)
    {
        _statusText.text = "Real robot available, press join to connect.";
    }
    
    private void DisplayError(string error)
    {
        _statusText.text = error;
        Debug.LogWarning(error);
    }
    
    private void OnDestroy()
    {
        _joinButton.onClick.RemoveListener(() =>
        {
            _statusText.text = "";
        });
        
        GlobalClient.Instance.OnSessionJoin -= DisplayAvailability;
        GlobalClient.Instance.OnSessionJoinFailed -= DisplayError;
    }
    
    /// <summary>
    /// Loads a scene asynchronously.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Start loading the scene
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        
        // Wait until the scene is fully loaded
        while (asyncLoad is { isDone: false })
        {
            yield return null;
        }
    }

}
