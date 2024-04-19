using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TaskMenuController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject normalObjective;
    public GameObject sliderObjective;
    public GameObject listElement;

    [Header("References")]
    public GameObject viewportContent;
    public GameObject objectivesList;
    public GameObject pendantUI;
    
    private TaskData[] tasks;

    public void Enter(TaskData[] taskData)
    {
        tasks = taskData;
        
        FillTaskList();
        
        pendantUI.SetActive(true);
    }

    public void Exit()
    {
        pendantUI.SetActive(false);

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
public struct ObjectiveData
{
    public string name;
    public Constraint constraint;
}
