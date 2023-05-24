using UnityEngine;

public class PolyscopeMoveJointUI : MonoBehaviour
{
    public RobotTranslator robotTranslator;

    private void Start()
    {
        if (robotTranslator == null)
        {
            Debug.LogWarning("Cannot render Move Joint UI, RobotTranslator is not assigned");
            return;
        }

        PolyscopeMoveJointElement[] polyscopeMoveElements = GetComponentsInChildren<PolyscopeMoveJointElement>();
        for (int i = 0; i < polyscopeMoveElements.Length; i++)
        {
            polyscopeMoveElements[i].jointIndex = i;
        }
    }
}
