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

using Schema.Socket.Unity;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

#endregion

namespace VNCScreen
{
    /// <summary>
    /// Represents a component that acts as a mouse cursor to the VncScreen using the XRTK interactor.
    /// This component should be attached to a small sphere or any other object.
    /// </summary>
    public class VNCXRRaycaster : MonoBehaviour
    {
        private Vector2 _textureCoord;
        private XRRayInteractor _xrRayInteractor;
        private bool _isHitting;

        public Vector2D TextureCoord
        {
            get
            {
                var vector2D = new Vector2D
                {
                    X = _textureCoord.x,
                    Y = _textureCoord.y
                };
                return vector2D;
            }
        }

        public bool IsHitting => _isHitting;

        /// <summary>
        /// The Awake method is called when the script instance is being loaded.
        /// It initializes the XRRayInteractor component.
        /// </summary>
        private void Awake()
        {
            _xrRayInteractor = GetComponent<XRRayInteractor>();
            _textureCoord = new Vector2();
        }

        /// <summary>
        /// The Update method is called every frame, if the MonoBehaviour is enabled.
        /// It checks if the XRRayInteractor is null and if it's not, it tries to get the current raycast.
        /// If the raycast hits a VNCScreen, it updates the mouse position and click status on the VNCScreen.
        /// </summary>
        private void Update()
        {
            if (_xrRayInteractor is null)
            {
                Debug.Log("XRRayInteractor is null. Make sure to have a XRRayInteractor assigned to this component.");
                return;
            }

            if (_xrRayInteractor.TryGetCurrentRaycast(out var raycastHit, out _, out _, out _, out _))
            {
                if (!raycastHit.HasValue || !raycastHit.Value.collider) return;

                raycastHit.Value.collider.TryGetComponent<VNCScreen>(out var vnc);
                if (vnc)
                {
                    // Check if user has permission to control the robot
                    if (SessionClient.Instance?.PendantOwner != SessionClient.Instance?.ClientId) return;

                    _isHitting = true;
                    _textureCoord = raycastHit.Value.textureCoord;
                    vnc.UpdateMouse(_textureCoord, _xrRayInteractor.isSelectActive);
                }
                else // If the raycast hits something that is not a VNCScreen, set the isHitting to false
                {
                    _isHitting = false;
                }
            }
        }
    }
}