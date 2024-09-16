using UnityEngine;

public class ResetPos : MonoBehaviour
{
    public Transform resetPosition;

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Respawn"))
        {
            transform.position = resetPosition.position;
            transform.rotation = resetPosition.rotation;
            transform.localScale = resetPosition.localScale;
        }
    }
}
