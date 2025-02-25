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
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

#endregion

/// <summary>
/// Manages the task menu, including displaying tasks and handling user interactions.
/// </summary>
public class TaskMenuController : MonoBehaviour
{
    [Header("Prefabs")] public GameObject normalObjective;
    public GameObject sliderObjective;
    public GameObject listElement;
    public GameObject multipleChoiceElement;

    [Header("References")] public GameObject viewportContent;
    public GameObject objectivesList;
    public GameObject pendantUI;
    public TMP_Text SidePanelHeader;

    private TaskData[] tasks;

    /// <summary>
    /// Called when the task menu is entered, initializing the task data.
    /// </summary>
    /// <param name="taskData">The array of task data to be displayed.</param>
    public void Enter(TaskData[] taskData)
    {
        tasks = taskData;

        FillTaskList();

        // pendantUI.SetActive(true);

        testMultipleChoice();
    }

    /// <summary>
    /// Called when the task menu is exited, cleaning up resources.
    /// </summary>
    public void Exit()
    {
        // pendantUI.SetActive(false);

        for (var i = viewportContent.transform.childCount; i > 0; i--)
            Destroy(viewportContent.transform.GetChild(i - 1).GameObject());
    }

    /// <summary>
    /// Fills the task list with the provided task data.
    /// </summary>
    private void FillTaskList()
    {
        for (var i = 0; i < tasks.Length; i++)
        {
            var _element = Instantiate(listElement, viewportContent.transform);
            var _text = _element.GetComponentInChildren<TMP_Text>();
            _text.text = tasks[i].name;

            var _button = _element.GetComponentInChildren<Button>();
            _button.onClick.AddListener(() => ChangeObjectives(i));
        }
    }

    /// <summary>
    /// Changes the objectives displayed based on the selected task index.
    /// </summary>
    /// <param name="index">The index of the selected task.</param>
    public void ChangeObjectives(int index)
    {
        SidePanelHeader.text = "Current objectives:";

        FillObjectivesList(index);
        //TODO: reset progress/reset robot???
    }

    /// <summary>
    /// Fills the objectives list with the objectives of the selected task.
    /// </summary>
    /// <param name="index">The index of the selected task.</param>
    public void FillObjectivesList(int index)
    {
        foreach (var task in tasks[index].objectives)
        {
            var _element = Instantiate(normalObjective, objectivesList.transform);
            var text = _element.GetComponentInChildren<TMP_Text>();
            text.text = task.name;
        }
    }

    public void testMultipleChoice()
    {
        var data = new MultipleChoiceData();
        data.options = new[] { "Test", "Another option", "The last option" };
        FillMultipleChoice(data);
    }

    public void FillMultipleChoice(MultipleChoiceData data)
    {
        SidePanelHeader.text = "Select an option:";

        var i = 65;

        foreach (var option in data.options)
        {
            var _element = Instantiate(multipleChoiceElement, objectivesList.transform);
            var text = _element.GetComponentInChildren<TMP_Text>();
            text.text = option;
            _element.GetNamedChild("OptionLetter").GetComponent<TMP_Text>().text = char.ConvertFromUtf32(i);
            i++;
        }
    }
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TaskMenuData", order = 1)]
public class TaskMenuData : ScriptableObject
{
    public TaskData[] data;
}

[Serializable]
public struct TaskData
{
    public string name;
    public ObjectiveData[] objectives;
}

[Serializable]
public struct MultipleChoiceData
{
    public string[] options;
}

[Serializable]
public struct ObjectiveData
{
    public string name;
}