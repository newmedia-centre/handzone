using System;
using System.Collections.Generic;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    public Action<SectionData> OnSectionSelected;
    public Action<ChapterData> OnChapterSelected;
    
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
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<MenuController>();
                    singletonObject.name = typeof(MenuController).ToString() + " (Singleton)";
                }
            }

            return _instance;
        }
    }
    
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
        if(loginMenu) _menuDictionary.Add(MenuName.Login, loginMenu);
        if(mainMenu) _menuDictionary.Add(MenuName.Main, mainMenu);
        if(virtualRobotMenu) _menuDictionary.Add(MenuName.VirtualRobot, virtualRobotMenu);
        if(realRobotMenu) _menuDictionary.Add(MenuName.RealRobot, realRobotMenu);
        if(tutorialMenu) _menuDictionary.Add(MenuName.Tutorial, tutorialMenu);
        if(exercisesMenu) _menuDictionary.Add(MenuName.Exercises, exercisesMenu);
        if(playbackMenu) _menuDictionary.Add(MenuName.Playback, playbackMenu);
        
        OnSectionSelected += OnSectionSelectedHandler;
        OnChapterSelected += OnChapterSelectedHandler;

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
    

    private void OnSectionSelectedHandler(SectionData obj)
    {
        currentSelectedSection = obj;
    }
    
    private void OnChapterSelectedHandler(ChapterData obj)
    {
        currentSelectedChapter = obj;
    }
}
