using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[Serializable]
public enum MenuName
{
    Login,
    Main,
    Options,
    RealRobotSession,
    VirtualRobotSessions
}

public class MainMenuController : MonoBehaviour
{
    public GameObject loginMenu;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject realRobotMenu;
    public GameObject sessionsMenu;
    
    // Variable to store the previous menu
    private MenuName _previousMenu;

    private Dictionary<MenuName, GameObject> menuDictionary = new();

    private void Start()
    {
        menuDictionary.Add(MenuName.Login, loginMenu);
        menuDictionary.Add(MenuName.Main, mainMenu);
        menuDictionary.Add(MenuName.Options, optionsMenu);
        menuDictionary.Add(MenuName.RealRobotSession, realRobotMenu);
        menuDictionary.Add(MenuName.VirtualRobotSessions, sessionsMenu);

        ChangeMenu(MenuName.Main);
    }

    public void SetScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName) != null)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene " + sceneName + " does not exist.");
        }
    }

    public void ChangeMenu(MenuControllerOption menuName)
    {
        _previousMenu = menuName.menuName;
        
        foreach (var menu in menuDictionary)
        {
            menu.Value.SetActive(menu.Key == menuName.menuName);
        }
    }
    
    public void ChangeMenu(MenuName menuName)
    {
        _previousMenu = menuName;
        
        foreach (var menu in menuDictionary)
        {
            menu.Value.SetActive(menu.Key == menuName);
        }
    }
    
    public void GoBack()
    {
        ChangeMenu(_previousMenu);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
