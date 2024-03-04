using Unity.XR.CoreUtils;
using UnityEngine;

public class GripperRotator : MonoBehaviour
{
    public GameObject player;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 delta = transform.position - player.transform.position;

        transform.rotation = Quaternion.LookRotation(delta);
    }
}
