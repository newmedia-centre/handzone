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
    public Button loginButton;
    //public TMP_InputField pinInputField;
    public TMP_Text pinInputField;

    public UnityEvent OnConnectionSuccess;
    
    private void Start()
    {
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
            });
        };
    }
    
    public async void GlobalClientLogin()
    {
        string pin = await GlobalClient.Instance.GetPin();
        if (string.IsNullOrEmpty(pin))
        {
            Debug.LogError("PIN is empty.");
            return;
        }

        pinInputField.text = pin;
        Debug.Log(pin);
        
        try
        {
            await GlobalClient.Instance.TryConnectToGlobalServer(pin);
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
        }
    }
}
