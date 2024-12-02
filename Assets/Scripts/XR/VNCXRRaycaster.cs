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