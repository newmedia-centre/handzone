using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuDocument : MonoBehaviour
{
    public UIDocument mainmenuDocument;

    private Button _tutorialButton;
    private Button _exercisesButton;
    private Button _virtualRobotButton;
    private Button _realRobotButton;
    private Button _optionsButton;
    private Button _quitButton;
    
    private void Awake()
    {
        mainmenuDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _tutorialButton = mainmenuDocument.rootVisualElement.Q<Button>("TutorialButton");
        _exercisesButton = mainmenuDocument.rootVisualElement.Q<Button>("ExercisesButton");
        _virtualRobotButton = mainmenuDocument.rootVisualElement.Q<Button>("VirtualRobotButton");
        _realRobotButton = mainmenuDocument.rootVisualElement.Q<Button>("RealRobotButton");
        _optionsButton = mainmenuDocument.rootVisualElement.Q<Button>("OptionsButton");
        _quitButton = mainmenuDocument.rootVisualElement.Q<Button>("QuitButton");
        
        if (_tutorialButton == null || _exercisesButton == null || _virtualRobotButton == null || _realRobotButton == null || _optionsButton == null || _quitButton == null)
        {
            Debug.LogError("MainMenuDocument: One or more UI elements are not found.");
            return;
        }
        
        _tutorialButton.clicked += OnTutorialButtonClicked;
        _exercisesButton.clicked += OnExercisesButtonClicked;
        _virtualRobotButton.clicked += OnVirtualRobotButtonClicked;
        _realRobotButton.clicked += OnRealRobotButtonClicked;
        _quitButton.clicked += OnQuitButtonClicked;
    }

    private void OnQuitButtonClicked()
    {
        MenuController.Instance.QuitGame();
    }

    private void OnRealRobotButtonClicked()
    {
        MenuController.Instance.ChangeMenu(MenuName.RealRobot);
    }

    private void OnVirtualRobotButtonClicked()
    {
        MenuController.Instance.ChangeMenu(MenuName.VirtualRobot);
    }

    private void OnExercisesButtonClicked()
    {
        MenuController.Instance.ChangeMenu(MenuName.Exercises);
    }

    private void OnTutorialButtonClicked()
    {
        MenuController.Instance.ChangeMenu(MenuName.Tutorial);
    }
}
