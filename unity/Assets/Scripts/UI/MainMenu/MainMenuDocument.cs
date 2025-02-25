/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

#region

using UnityEngine;
using UnityEngine.UIElements;

#endregion

/// <summary>
/// Manages the UI elements of the main menu, including button interactions and menu navigation.
/// </summary>
public class MainMenuDocument : MonoBehaviour
{
    public UIDocument mainmenuDocument;

    private Button _tutorialButton;
    private Button _exercisesButton;
    private Button _virtualRobotButton;
    private Button _realRobotButton;
    private Button _optionsButton;
    private Button _quitButton;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the main menu document.
    /// </summary>
    private void Awake()
    {
        mainmenuDocument = GetComponent<UIDocument>();
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Sets up button references and event listeners.
    /// </summary>
    private void OnEnable()
    {
        _tutorialButton = mainmenuDocument.rootVisualElement.Q<Button>("TutorialButton");
        _exercisesButton = mainmenuDocument.rootVisualElement.Q<Button>("ExercisesButton");
        _virtualRobotButton = mainmenuDocument.rootVisualElement.Q<Button>("VirtualRobotButton");
        _realRobotButton = mainmenuDocument.rootVisualElement.Q<Button>("RealRobotButton");
        _optionsButton = mainmenuDocument.rootVisualElement.Q<Button>("OptionsButton");
        _quitButton = mainmenuDocument.rootVisualElement.Q<Button>("QuitButton");

        if (_tutorialButton == null || _exercisesButton == null || _virtualRobotButton == null ||
            _realRobotButton == null || _optionsButton == null || _quitButton == null)
        {
            Debug.LogError("MainMenuDocument: One or more UI elements are not found.");
            return;
        }

        // Disable buttons, until they are implemented
        _exercisesButton.SetEnabled(false);
        _exercisesButton.focusable = false;
        _optionsButton.SetEnabled(false);
        _optionsButton.focusable = false;

        _tutorialButton.clicked += OnTutorialButtonClicked;
        _exercisesButton.clicked += OnExercisesButtonClicked;
        _virtualRobotButton.clicked += OnVirtualRobotButtonClicked;
        _realRobotButton.clicked += OnRealRobotButtonClicked;
        _quitButton.clicked += OnQuitButtonClicked;
    }

    /// <summary>
    /// Handles the click event for the quit button.
    /// </summary>
    private void OnQuitButtonClicked()
    {
        MenuController.Instance.QuitGame();
    }

    /// <summary>
    /// Handles the click event for the real robot button.
    /// </summary>
    private void OnRealRobotButtonClicked()
    {
        MenuController.Instance.ChangeMenu(MenuName.RealRobot);
    }

    /// <summary>
    /// Handles the click event for the virtual robot button.
    /// </summary>
    private void OnVirtualRobotButtonClicked()
    {
        MenuController.Instance.ChangeMenu(MenuName.VirtualRobot);
    }

    /// <summary>
    /// Handles the click event for the exercises button.
    /// </summary>
    private void OnExercisesButtonClicked()
    {
        MenuController.Instance.ChangeMenu(MenuName.Exercises);
    }

    /// <summary>
    /// Handles the click event for the tutorial button.
    /// </summary>
    private void OnTutorialButtonClicked()
    {
        MenuController.Instance.ChangeMenu(MenuName.Tutorial);
    }
}