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