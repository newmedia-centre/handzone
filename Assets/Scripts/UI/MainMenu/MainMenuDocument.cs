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