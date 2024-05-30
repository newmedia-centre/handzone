using System;
using PimDeWitte.UnityMainThreadDispatcher;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GlobalClientLoginButton : MonoBehaviour
{
    public Button loginButton;
    public TMP_InputField pinInputField;
    
    private void Start()
    {
        if(loginButton == null)
            loginButton = GetComponent<Button>();
        loginButton.onClick.AddListener(GlobalClientLogin);
        
        GlobalClient.Instance.OnConnecting += () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                loginButton.interactable = false;
                pinInputField.interactable = false;
                loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connecting...";
            });
        };
        
        GlobalClient.Instance.OnConnected += () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                loginButton.interactable = true;
                pinInputField.text = "";
                pinInputField.interactable = false;
                loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connected";
            });
        };
        
        GlobalClient.Instance.OnDisconnected += () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                loginButton.interactable = true;
                pinInputField.interactable = true;
                loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Login";
            });
        };
        
        GlobalClient.Instance.OnError += (error) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                loginButton.interactable = true;
                pinInputField.interactable = true;
                loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Login";
                Debug.LogError($"An error occurred: {error}");
            });
        };
    }
    
    public async void GlobalClientLogin()
    {
        string pin = pinInputField.text;
        if (string.IsNullOrEmpty(pin))
        {
            Debug.LogError("PIN is empty.");
            return;
        }
        
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
