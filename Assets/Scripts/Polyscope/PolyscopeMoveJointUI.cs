using UnityEngine;

public class PolyscopeMoveJointUI : MonoBehaviour
{
    public PolyscopeRobot polyscopeRobot;
    public GameObject moveJointElementPrefab;
    public float elementOffset = -25.0f;
    
    private void Start()
    {
        if (polyscopeRobot == null)
        {
            Debug.LogWarning("Cannot render Move Joint UI, PolyscopeRobot is not assigned");
            return;
        }
        
        var jointTransformAndAxisList = polyscopeRobot.GetJointTransformsAndEnabledRotationAxis();
        
        var offset = Vector3.zero;
        for (int i = 0; i < jointTransformAndAxisList.Count; i++)
        {
            var element = Instantiate(moveJointElementPrefab, transform);
            offset.y = elementOffset * i;
            element.transform.localPosition += (offset);
            var jointElement = element.GetComponent<PolyscopeMoveJointElement>();
            jointElement.jointInfo = jointTransformAndAxisList[i];
        }
    }
}
