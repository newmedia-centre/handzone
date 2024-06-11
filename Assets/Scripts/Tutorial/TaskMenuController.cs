using System;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class TaskMenuController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject normalObjective;
    public GameObject sliderObjective;
    public GameObject listElement;
    public GameObject multipleChoiceElement;

    [Header("References")]
    public GameObject viewportContent;
    public GameObject objectivesList;
    public GameObject pendantUI;
    public TMP_Text SidePanelHeader;
    
    private TaskData[] tasks;

    public void Enter(TaskData[] taskData)
    {
        tasks = taskData;
        
        FillTaskList();
        
        // pendantUI.SetActive(true);
        
        testMultipleChoice();
    }

    public void Exit()
    {
        // pendantUI.SetActive(false);

        for (int i = viewportContent.transform.childCount; i > 0; i--)
        {
            Destroy(viewportContent.transform.GetChild(i - 1).GameObject());
        }
    }
    
    private void FillTaskList()
    {
        for (int i = 0; i < tasks.Length; i++)
        {
            GameObject _element = Instantiate(listElement, viewportContent.transform);
            TMP_Text _text = _element.GetComponentInChildren<TMP_Text>();
            _text.text = tasks[i].name;

            Button _button = _element.GetComponentInChildren<Button>();
            _button.onClick.AddListener(() => ChangeObjectives(i));
        }
    }

    public void ChangeObjectives(int index)
    {
        SidePanelHeader.text = "Current objectives:";
        
        FillObjectivesList(index);
        //TODO: reset progress/reset robot???
    }

    public void FillObjectivesList(int index)
    {
        foreach(ObjectiveData task in tasks[index].objectives)
        {
            GameObject _element = Instantiate(normalObjective, objectivesList.transform);
            TMP_Text text = _element.GetComponentInChildren<TMP_Text>();
            text.text = task.name;
        }
    }

    public void testMultipleChoice()
    {
        MultipleChoiceData data = new MultipleChoiceData();
        data.options = new[] { "Test", "Another option", "The last option" };
        FillMultipleChoice(data);
    }

    public void FillMultipleChoice(MultipleChoiceData data)
    {
        SidePanelHeader.text = "Select an option:";
        
        int i = 65;
        
        foreach(string option in data.options)
        {
            GameObject _element = Instantiate(multipleChoiceElement, objectivesList.transform);
            TMP_Text text = _element.GetComponentInChildren<TMP_Text>();
            text.text = option;
            _element.GetNamedChild("OptionLetter").GetComponent<TMP_Text>().text = Char.ConvertFromUtf32(i);
            i++;
        }
    }
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TaskMenuData", order = 1)]
public class TaskMenuData : ScriptableObject
{
    public TaskData[] data;
}

[System.Serializable]
public struct TaskData
{
    public string name;
    public ObjectiveData[] objectives;
}

[System.Serializable]
public struct MultipleChoiceData
{
    public string[] options;
}

[System.Serializable]
public struct ObjectiveData
{
    public string name;
}
