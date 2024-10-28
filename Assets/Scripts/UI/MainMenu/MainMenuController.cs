using System;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject loginMenu;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject sessionsMenu;
    public GameObject realRobotMenu;
    
    // Variable to store the previous menu
    private MenuName _previousMenu;
    private MenuName _currentMenu;

    private Dictionary<MenuName, GameObject> _menuDictionary = new();

    private void Start()
    {
        _menuDictionary.Add(MenuName.Login, loginMenu);
        _menuDictionary.Add(MenuName.Main, mainMenu);
        _menuDictionary.Add(MenuName.Options, optionsMenu);
        _menuDictionary.Add(MenuName.VirtualRobot, sessionsMenu);
        _menuDictionary.Add(MenuName.RealRobot, realRobotMenu);

        ChangeMenu(MenuName.Login);
    }

    public void ChangeMenu(MenuControllerOption menuName)
    {
        ChangeMenu(menuName.menuName);
    }
    
    public void ChangeMenu(MenuName menuName)
    {
        _previousMenu = _currentMenu;
        _currentMenu = menuName;
        foreach (var menu in _menuDictionary)
        {
            menu.Value.SetActive(menu.Key == _currentMenu);
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
