using System;
using PimDeWitte.UnityMainThreadDispatcher;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GlobalClientLoginButton : MonoBehaviour
{
    public MainMenuController mainMenuController;
    public Button loginButton;
    //public TMP_InputField pinInputField;
    public TMP_Text pinInputField;
    public GameObject logPanel;
    
    private MenuControllerOption _menuControllerOption;
    private TMP_Text _logText;
    public UnityEvent OnConnectionSuccess;
    
    private void Start()
    {
        if(mainMenuController == null)
            mainMenuController = FindObjectOfType<MainMenuController>();

        TryGetComponent(out _menuControllerOption);
        
        if(logPanel == null)
            logPanel = GameObject.Find("LogPanel");
        
        _logText = logPanel.GetComponentInChildren<TMP_Text>();
        logPanel.SetActive(false);
        _logText.text = "";
        
        if(loginButton == null)
            loginButton = GetComponent<Button>();
        loginButton.onClick.AddListener(GlobalClientLogin);
        
        GlobalClient.Instance.OnConnecting += () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if(loginButton == null || pinInputField == null)
                    return;
                
                loginButton.interactable = false;
                //pinInputField.interactable = false;
                loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connecting...";
            });
        };
        
        GlobalClient.Instance.OnConnected += () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if(loginButton == null || pinInputField == null)
                    return;
                
                loginButton.interactable = true;
                pinInputField.text = "";
                //pinInputField.interactable = false;
                loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connected";
                OnConnectionSuccess.Invoke();
            });
        };
        
        GlobalClient.Instance.OnDisconnected += () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if(loginButton == null || pinInputField == null)
                    return;
                
                loginButton.interactable = true;
                //pinInputField.interactable = true;
                loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Login";
            });
        };
        
        GlobalClient.Instance.OnError += (error) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if(loginButton == null || pinInputField == null)
                    return;
                
                loginButton.interactable = true;
                //pinInputField.interactable = true;
                loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Login";
                Debug.LogError($"An error occurred: {error}");
                _logText.text = $"An error occurred: {error}";
            });
        };
    }
    
    /// <summary>
    /// Button click event to login to the global server. It makes sure to check if the PIN is empty.
    /// It also changes the main menu.
    /// </summary>
    private async void GlobalClientLogin()
    {
        string pin = await GlobalClient.Instance.GetPin();
        if (string.IsNullOrEmpty(pin))
        {
            Debug.LogWarning("PIN is empty.");
            logPanel.SetActive(true);
            _logText.text = "Could not get PIN from server.";
            return;
        }

        pinInputField.text = pin;
        Debug.Log(pin);

        if (logPanel.activeSelf)
            logPanel.SetActive(false);
        
        try
        {
            await GlobalClient.Instance.TryConnectToGlobalServer(pin);
            mainMenuController.ChangeMenu(_menuControllerOption);
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
            logPanel.SetActive(true);
            _logText.text = $"An error occurred: {ex.Message}";
        }
    }
}
