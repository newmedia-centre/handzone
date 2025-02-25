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