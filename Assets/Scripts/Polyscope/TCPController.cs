using UnityEngine;

public class TCPController : MonoBehaviour
{
    public float translationSpeed = 3.0f;
    public float rotationSpeed = 8.0f;

    public void TranslateObject(Vector3 direction)
    {
        transform.Translate(direction * (translationSpeed * Time.deltaTime), Space.World);
    }

    public void RotateObject(Vector3 axis)
    {
        transform.Rotate(axis * (rotationSpeed * Time.deltaTime), Space.World);
    }
}
