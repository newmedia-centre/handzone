using Schema.Socket.Unity;
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
        
        private Vector2 _textureCoord;  
        private XRRayInteractor _xrRayInteractor;
        
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

        /// <summary>
        /// The Awake method is called when the script instance is being loaded.
        /// It initializes the XRRayInteractor component.
        /// </summary>
        void Awake()
        {
            _xrRayInteractor = GetComponent<XRRayInteractor>();
            _textureCoord = new Vector2();
        }

        /// <summary>
        /// The Update method is called every frame, if the MonoBehaviour is enabled.
        /// It checks if the XRRayInteractor is null and if it's not, it tries to get the current raycast.
        /// If the raycast hits a VNCScreen, it updates the mouse position and click status on the VNCScreen.
        /// </summary>
        void Update()
        {
            if (_xrRayInteractor == null || SessionClient.Instance == null)
                return;
            
            // Check if user has permission to control the robot
            if(SessionClient.Instance.PendantOwner != SessionClient.Instance.ClientId)
                return;
            
            if (_xrRayInteractor.TryGetCurrentRaycast(out var raycastHit, out _, out _, out _, out _))
            {
                if (raycastHit != null)
                {
                    raycastHit.Value.collider.TryGetComponent<VNCScreen>(out var vnc);
                    if(vnc != null)
                    {
                        _textureCoord = raycastHit.Value.textureCoord;
                        vnc.UpdateMouse(_textureCoord, _xrRayInteractor.isSelectActive);
                    }
                }
            }
        }
    }
}