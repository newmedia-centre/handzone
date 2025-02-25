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
using UnityEngine.InputSystem;

#endregion

namespace Robots.Samples.Unity
{
    public class CameraSimple : MonoBehaviour
    {
        private Vector3 _target;
        [Range(0.1f, 10.0f)] public float Sensitivity;
        public GameObject Menu;
        private Vector3 CamStartLoc;
        private Quaternion RotReset = new(0, 0, 0, 0);

        private void Start()
        {
            CamStartLoc = transform.position;
        }

        private void Update()
        {
            _target = transform.GetChild(0).transform.position;
            var mouse = Mouse.current;
            var mouseRightdown = mouse.rightButton.isPressed;
            var mouseScroll = mouse.scroll.ReadValue();
            var keyShift = Input.GetKey("left shift");

            if (mouseRightdown == true && keyShift == false)
            {
                var delta = mouse.delta.ReadValue() * (Sensitivity / 25);

                transform.RotateAround(_target, Vector3.up, delta.x);
                transform.RotateAround(_target, transform.rotation * Vector3.right, -delta.y);
            }

            var shouldZoom = mouseScroll.y != 0;

            if (shouldZoom)
            {
                var delta = Mathf.Sign(mouseScroll.y) * 0.1f;
                var distance = (_target - transform.position).magnitude * delta;
                transform.Translate(Vector3.forward * distance, Space.Self);
                //Finite Zoom Towards Focus, Requires development of Refocus on objects
                //transform.GetChild(0).transform.Translate(Vector3.back * distance, Space.Self);
            }

            if (mouseRightdown == true && keyShift == true)
            {
                Vector3 PanTranslation = new(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0);
                transform.Translate(PanTranslation * (Sensitivity / 100));
            }

            if (Input.GetKeyDown(KeyCode.E)) Menu.SetActive(!Menu.activeSelf);

            if (Input.GetKeyDown(KeyCode.V))
            {
                transform.position = CamStartLoc;
                transform.rotation = RotReset;
            }
        }
    }
}