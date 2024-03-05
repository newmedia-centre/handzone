using UnityEngine;

public class GripperRotator : MonoBehaviour
{
    public GameObject player;
    
    void Update()
    {
        Vector3 delta = transform.position - player.transform.position;

        transform.rotation = Quaternion.LookRotation(delta);
    }
}
