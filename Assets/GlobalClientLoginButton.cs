using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GlobalClientLoginButton : MonoBehaviour
{
    public Button loginButton;
    public TMP_InputField pinInputField;
    
    private void Start()
    {
        loginButton = GetComponent<Button>();
        loginButton.onClick.AddListener(GlobalClientLogin);
        
        GlobalClient.Instance.OnConnecting += () =>
        {
            loginButton.interactable = false;
            pinInputField.interactable = false;
            loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connecting...";
        };
        
        GlobalClient.Instance.OnConnected += () =>
        {
            loginButton.interactable = true;
            pinInputField.text = "";
            pinInputField.interactable = false;
            loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connected";
        };
        
        GlobalClient.Instance.OnDisconnected += () =>
        {
            loginButton.interactable = true;
            pinInputField.interactable = true;
            loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Login";
        };
        
        GlobalClient.Instance.OnError += (error) =>
        {
            loginButton.interactable = true;
            pinInputField.interactable = true;
            loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Login";
            Debug.LogError($"An error occurred: {error}");
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
            await GlobalClient.Instance.TryConnectToWebServer(pin);
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
        }
    }
}
