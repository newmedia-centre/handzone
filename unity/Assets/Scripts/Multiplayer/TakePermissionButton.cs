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
using UnityEngine.UI;

#endregion

/// <summary>
/// The TakePermissionButton class manages the functionality of a button that
/// requests control permission for a robotic session in a multiplayer environment.
/// It initializes the button's click event to trigger a permission request and
/// handles the cleanup of event listeners when the object is destroyed.
/// </summary>
public class TakePermissionButton : MonoBehaviour
{
    private Button _button;

    /// <summary>
    /// Initializes the TakePermissionButton by setting up the button's click event
    /// to request permission when clicked. It also logs a warning if the button
    /// component is not found.
    /// </summary>
    private void Start()
    {
        if (TryGetComponent(out _button))
            _button.onClick.AddListener(RequestPermission);
        else
            Debug.LogWarning("Button component not found.");
    }

    /// <summary>
    /// Requests control permission from the session client. If the session client
    /// instance is available, it calls the TakeControlPermission method.
    /// </summary>
    private void RequestPermission()
    {
        if (SessionClient.Instance)
            SessionClient.Instance.TakeControlPermission();
    }

    /// <summary>
    /// Cleans up the button's event listeners when the object is destroyed,
    /// ensuring that there are no lingering references.
    /// </summary>
    private void OnDestroy()
    {
        _button.onClick.RemoveListener(RequestPermission);
    }
}