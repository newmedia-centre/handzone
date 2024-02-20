using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject multiplayerMenu;

    public void SetScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SetMainMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
    }

    public void SetOptionsMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        multiplayerMenu.SetActive(false);
    }

    public void SetMultiplayerMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
