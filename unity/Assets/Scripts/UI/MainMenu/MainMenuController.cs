// Copyright 2024 NewMedia Centre - Delft University of Technology
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#region

using System.Collections.Generic;
using UnityEngine;

#endregion

/// <summary>
/// Manages the main menu, including menu transitions and menu state handling.
/// </summary>
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

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the menu dictionary and sets the initial menu.
    /// </summary>
    private void Start()
    {
        _menuDictionary.Add(MenuName.Login, loginMenu);
        _menuDictionary.Add(MenuName.Main, mainMenu);
        _menuDictionary.Add(MenuName.Options, optionsMenu);
        _menuDictionary.Add(MenuName.VirtualRobot, sessionsMenu);
        _menuDictionary.Add(MenuName.RealRobot, realRobotMenu);

        ChangeMenu(MenuName.Login);
    }

    /// <summary>
    /// Changes the current menu to the specified menu name.
    /// </summary>
    /// <param name="menuName">The name of the menu to switch to.</param>
    public void ChangeMenu(MenuControllerOption menuName)
    {
        ChangeMenu(menuName.menuName);
    }

    /// <summary>
    /// Changes the current menu to the specified menu name.
    /// </summary>
    /// <param name="menuName">The name of the menu to switch to.</param>
    public void ChangeMenu(MenuName menuName)
    {
        _previousMenu = _currentMenu;
        _currentMenu = menuName;
        foreach (var menu in _menuDictionary) menu.Value.SetActive(menu.Key == _currentMenu);
    }

    /// <summary>
    /// Navigates back to the previous menu.
    /// </summary>
    public void GoBack()
    {
        ChangeMenu(_previousMenu);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}