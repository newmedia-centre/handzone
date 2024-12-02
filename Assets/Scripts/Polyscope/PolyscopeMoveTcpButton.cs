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
using UnityEngine.EventSystems;

#endregion

/// <summary>
/// The PolyscopeMoveTcpButton class manages the interaction of a button that
/// controls the movement of the TCP (Tool Center Point).
/// It implements the IPointerDownHandler and IPointerUpHandler interfaces to manage
/// the button's pressed state, enabling or disabling the movement of the TCP
/// based on user input. The class also handles the direction of movement and
/// communicates with the TCPController to execute the movement commands.
/// </summary>
public class PolyscopeMoveTcpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] public TCPController tcpController;
    public Vector3 translateDirection;
    public Vector3 rotateAxis;

    private bool _isHeld;

    /// <summary>
    /// Called when the button is pressed down. It sets the held state to true
    /// and initiates the movement of the TCP.
    /// </summary>
    /// <param name="eventData">The pointer event data associated with the button press.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        _isHeld = true;
    }

    /// <summary>
    /// Called when the button is released. It sets the held state to false
    /// and stops the movement of the TCP.
    /// </summary>
    /// <param name="eventData">The pointer event data associated with the button release.</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        _isHeld = false;
        UR_EthernetIPClient.ClearSendBuffer?.Invoke();
    }

    /// <summary>
    /// Updates the state of the TCP movement if the button is held down.
    /// This method is called once per frame.
    /// </summary>
    private void Update()
    {
        if (_isHeld) SessionClient.Instance.Speedl(translateDirection, rotateAxis, 0.1f, 0.1f);
    }
}