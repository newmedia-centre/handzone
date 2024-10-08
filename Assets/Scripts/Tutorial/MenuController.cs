using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MenuController : MonoBehaviour
{
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
    
    public Action<SectionData> OnSectionSelected;
    public Action<ChapterData> OnChapterSelected;
    public Action OnLessonStarted;
    
    public SectionData currentSelectedSection;
    public ChapterData currentSelectedChapter;
    
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
        
        OnSectionSelected += OnSectionSelectedHandler;
        OnChapterSelected += OnChapterSelectedHandler;
        OnLessonStarted += OnLessonStartedHandler;
    }

    private void OnLessonStartedHandler()
    {
        ShowPlaybackMenu();
    }

    private void OnSectionSelectedHandler(SectionData obj)
    {
        currentSelectedSection = obj;
    }
    
    private void OnChapterSelectedHandler(ChapterData obj)
    {
        currentSelectedChapter = obj;
    }

    [Header("Object references")]
    public TutorialMenuDocument tutorialMenu;
    public PlaybackMenuDocument playBackMenu;
    
    public void ShowTutorialMenu()
    {
        playBackMenu.gameObject.SetActive(false);
        tutorialMenu.gameObject.SetActive(true);
    }
    
    public void ShowPlaybackMenu()
    {
        tutorialMenu.gameObject.SetActive(false);
        playBackMenu.gameObject.SetActive(true);
    }
}
