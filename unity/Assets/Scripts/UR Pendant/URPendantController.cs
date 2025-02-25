/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

#region

using UnityEngine;

#endregion

public class URPendantController : MonoBehaviour
{
    [Header("References")] public GameObject playerCamera;

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
            var deltaAngle = Quaternion.LookRotation(playerCamera.transform.forward).eulerAngles.y - _initialYAngle;

            transform.position = playerCamera.transform.position +
                                 Quaternion.Euler(new Vector3(0, deltaAngle, 0)) * _relativePosition;
            transform.rotation = Quaternion.Euler(new Vector3(_initialPendantXAngle, _initialPendantYAngle + deltaAngle,
                _initialPendantZAngle));
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