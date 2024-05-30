using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VNCScreen
{
    /// <summary>
    /// Represents a component that acts as a mouse cursor to the VncScreen using the XRTK interactor.
    /// This component should be attached to a small sphere or any other object.
    /// </summary>
    public class VNCXRRaycaster : MonoBehaviour
    {
        private XRRayInteractor _xrRayInteractor;

        /// <summary>
        /// The Awake method is called when the script instance is being loaded.
        /// It initializes the XRRayInteractor component.
        /// </summary>
        void Awake()
        {
            _xrRayInteractor = GetComponent<XRRayInteractor>();
        }

        /// <summary>
        /// The Update method is called every frame, if the MonoBehaviour is enabled.
        /// It checks if the XRRayInteractor is null and if it's not, it tries to get the current raycast.
        /// If the raycast hits a VNCScreen, it updates the mouse position and click status on the VNCScreen.
        /// </summary>
        void Update()
        {
            if (_xrRayInteractor == null)
                return;

            if (_xrRayInteractor.TryGetCurrentRaycast(out var raycastHit, out _, out _, out _, out _))
            {
                if (raycastHit != null)
                {
                    raycastHit.Value.collider.TryGetComponent<VNCScreen>(out var vnc);
                    if(vnc != null)
                    {
                        var textureCoord = raycastHit.Value.textureCoord;
                        vnc.UpdateMouse(textureCoord, _xrRayInteractor.isSelectActive);
                        
                        // TODO: Change pendent payload to accept Vector2 & bool
                        // SessionClient.Instance.SendPendantData(textureCoord, _xrRayInteractor.isSelectActive);
                    }
                }
            }
        }
    }
}