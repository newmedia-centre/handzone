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

using System;
using System.Collections.Generic;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

#endregion

[Serializable]
public enum MenuName
{
    Login,
    Options,
    Main,
    VirtualRobot,
    RealRobot,
    Tutorial,
    Exercises,
    Playback
}

/// <summary>
/// Manages the different menus in the application, handling menu transitions and selections.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Object references")] 
    public GameObject loginMenu;
    public GameObject mainMenu;
    public GameObject virtualRobotMenu;
    public GameObject realRobotMenu;
    public GameObject tutorialMenu;
    public GameObject exercisesMenu;
    public GameObject playbackMenu;
    public SectionData currentSelectedSection;
    public ChapterData currentSelectedChapter;

    private MenuName _previousMenu;
    private MenuName _currentMenu;
    private Dictionary<MenuName, GameObject> _menuDictionary = new();
    private bool _isMenuOpen = false;

    public Action<SectionData> OnSectionSelected;
    public Action<ChapterData> OnChapterSelected;
    
    [Header("Controller Action")] 
    public InputActionReference ToggleMenuButton;

    private static MenuController _instance;

    public static MenuController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MenuController>();
                if (_instance == null)
                {
                    var singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<MenuController>();
                    singletonObject.name = typeof(MenuController).ToString() + " (Singleton)";
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the singleton instance and ensures it persists across scenes.
    /// </summary>
    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
        if (loginMenu) _menuDictionary.Add(MenuName.Login, loginMenu);
        if (mainMenu) _menuDictionary.Add(MenuName.Main, mainMenu);
        if (virtualRobotMenu) _menuDictionary.Add(MenuName.VirtualRobot, virtualRobotMenu);
        if (realRobotMenu) _menuDictionary.Add(MenuName.RealRobot, realRobotMenu);
        if (tutorialMenu) _menuDictionary.Add(MenuName.Tutorial, tutorialMenu);
        if (exercisesMenu) _menuDictionary.Add(MenuName.Exercises, exercisesMenu);
        if (playbackMenu) _menuDictionary.Add(MenuName.Playback, playbackMenu);

        OnSectionSelected += OnSectionSelectedHandler;
        OnChapterSelected += OnChapterSelectedHandler;
        ToggleMenuButton.action.performed += _ => ToggleMenu();

        if (GlobalClient.Instance)
        {
            GlobalClient.Instance.OnConnected += () =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => ChangeMenu(MenuName.Main));
            };

            GlobalClient.Instance.OnDisconnected += () =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => ChangeMenu(MenuName.Login));
            };
        }

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
        _isMenuOpen = true;
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

    /// <summary>
    /// Handles the selection of a section.
    /// </summary>
    /// <param name="obj">The selected section data.</param>
    private void OnSectionSelectedHandler(SectionData obj)
    {
        currentSelectedSection = obj;
    }

    /// <summary>
    /// Handles the selection of a chapter.
    /// </summary>
    /// <param name="obj">The selected chapter data.</param>
    private void OnChapterSelectedHandler(ChapterData obj)
    {
        currentSelectedChapter = obj;
    }

    public void CompleteSection()
    {
        currentSelectedSection.completed = true;
    }

    /// <summary>
    /// Hides all menus. But remembers the previous menu.
    /// </summary>
    public void HideMenu()
    {
        _previousMenu = _currentMenu;
        foreach (var menu in _menuDictionary) menu.Value.SetActive(false);
        _isMenuOpen = false;
    }
    
    /// <summary>
    /// Shows the previous menu.
    /// </summary>
    public void ShowMenu()
    {
        ChangeMenu(_previousMenu);
    }
    
    public void ToggleMenu()
    {
        if (_isMenuOpen)
        {
            HideMenu();
        }
        else
        {
            ShowMenu();
        }
    }
    
}