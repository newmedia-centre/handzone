using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GripperAnim : MonoBehaviour
{
    private Animator _animController;
    private Gripper _gripper;
    
    // Start is called before the first frame update
    void Start()
    {
        _animController = GetComponent<Animator>();
        
        // Temporary fix for workshop
        _gripper = GetComponent<Gripper>();
        _gripper.SetAnchorPosition(new Vector3(0, 0, -1.453f));
        
        WebClient.OnDigitalOutputChanged += SetGripperAnim;
    }

    private void SetGripperAnim(bool state)
    {
        _animController.SetBool("Gripping", !state);

        if (state == true)
        {
            _gripper.Grab();
        }
        else
        {
            _gripper.UnGrab();
        }
    }
}
