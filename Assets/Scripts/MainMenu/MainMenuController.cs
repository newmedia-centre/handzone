using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject loginMenu;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject multiplayerMenu;
    public GameObject sessionsMenu;
    
    private void Start()
    {
        SetLoginMenu();
    }

    public void SetScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeMenu(string menuName)
    {
        switch (menuName)
        {
            case "Login":
                SetLoginMenu();
                break;
            case "Main":
                SetMainMenu();
                break;
            case "Options":
                SetOptionsMenu();
                break;
            case "Multiplayer":
                SetMultiplayerMenu();
                break;
            case "Sessions":
                SetSessionsMenu();
                break;
        }
    }

    public void SetSessionsMenu()
    {
        sessionsMenu.SetActive(true);
        loginMenu.SetActive(false);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        
    }

    public void SetLoginMenu()
    {
        sessionsMenu.SetActive(false);
        loginMenu.SetActive(true);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
    }

    public void SetMainMenu()
    {
        sessionsMenu.SetActive(false);
        loginMenu.SetActive(false);
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
    }

    public void SetOptionsMenu()
    {
        sessionsMenu.SetActive(false);
        loginMenu.SetActive(false);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        multiplayerMenu.SetActive(false);
    }

    public void SetMultiplayerMenu()
    {
        sessionsMenu.SetActive(false);
        loginMenu.SetActive(false);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
