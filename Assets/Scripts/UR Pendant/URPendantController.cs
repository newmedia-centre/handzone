// Copyright 2024 NewMedia Centre - Delft University of Technology
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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