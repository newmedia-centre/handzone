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