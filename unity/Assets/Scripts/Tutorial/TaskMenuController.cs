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