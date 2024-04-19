using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Object references")]
    public GameObject chapterMenuObject;
    public GameObject tutorialMenuObject;
    public GameObject taskMenuObject;

    [Header("Menu Controller references")]
    public ChapterMenuController chapterController;
    public TutorialMenuController tutorialController;
    public TaskMenuController taskController;
    
    public void BackToChaptersFromTutorials()
    {
        tutorialMenuObject.SetActive(false);
        chapterMenuObject.SetActive(true);
        tutorialController.Exit();
    }
    
    public void GoToTutorials()
    {
        tutorialController.Enter(chapterController.currentChapterData.tutorialData.data);
        chapterMenuObject.SetActive(false);
        tutorialMenuObject.SetActive(true);
    }
    
    public void BackToChaptersFromTask()
    {
        taskMenuObject.SetActive(false);
        chapterMenuObject.SetActive(true);
        taskController.Exit();
    }
    
    public void GoToTasks()
    {
        taskController.Enter(chapterController.currentChapterData.tasks.data);
        chapterMenuObject.SetActive(false);
        taskMenuObject.SetActive(true);
    }
}
