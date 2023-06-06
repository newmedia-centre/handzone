using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SwitchDisplay : MonoBehaviour
{
    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            camera.targetDisplay = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            camera.targetDisplay = 1;
        }
    }
}
