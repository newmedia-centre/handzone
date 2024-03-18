using UnityEngine;

public class URPendantController : MonoBehaviour
{
    [Header("References")]
    public GameObject playerCamera;
    
    /**
     * Since Monobehavior already contains a rigidbody variable,
     * we need to tell the C# compiler that we want the variable name to refer to our variable.
    **/
    public new Rigidbody rigidbody;

    private bool _followingPlayer = false;
    private Vector3 _relativePosition;
    private float _initialYAngle;
    private float _initialPendantXAngle;
    private float _initialPendantYAngle;
    private float _initialPendantZAngle;

    public void Update()
    {
        if (_followingPlayer)
        {
            float deltaAngle = Quaternion.LookRotation(playerCamera.transform.forward).eulerAngles.y - _initialYAngle;

            transform.position = playerCamera.transform.position + Quaternion.Euler(new Vector3(0, deltaAngle, 0)) * _relativePosition;
            transform.rotation = Quaternion.Euler(new Vector3(_initialPendantXAngle, _initialPendantYAngle + deltaAngle, _initialPendantZAngle));
        }
    }

    public void ToggleFollowPlayer()
    {
        if (_followingPlayer)
        {
            rigidbody.isKinematic = false;
            _followingPlayer = false;
        }
        else
        {
            rigidbody.isKinematic = true;

            _relativePosition = transform.position - playerCamera.transform.position;
            _initialYAngle = Quaternion.LookRotation(playerCamera.transform.forward).eulerAngles.y;

            _initialPendantXAngle = transform.rotation.eulerAngles.x;
            _initialPendantYAngle = transform.rotation.eulerAngles.y;
            _initialPendantZAngle = transform.rotation.eulerAngles.z;

            _followingPlayer = true;
        }
    }
    
    //Triggers when the player stops grabbing the pendant
    public void SelectExit()
    {
        if (_followingPlayer)
        {
            _relativePosition = transform.position - playerCamera.transform.position;
            _initialYAngle = Quaternion.LookRotation(playerCamera.transform.forward).eulerAngles.y;

            _initialPendantXAngle = transform.rotation.eulerAngles.x;
            _initialPendantYAngle = transform.rotation.eulerAngles.y;
            _initialPendantZAngle = transform.rotation.eulerAngles.z;
        }
    }
}
