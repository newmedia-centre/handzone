using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RobotJointInteractable : XRBaseInteractable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        Debug.Log("Select entered");
    }
}
