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

using Schema.Socket.Index;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

/// <summary>
/// Represents a button in the session menu that allows users to select a session.
/// </summary>
public class SessionButton : MonoBehaviour
{
    private Button _button;
    private string _sessionAddress;
    private Color _originalColor;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the button and stores its original color.
    /// </summary>
    public void Start()
    {
        _button = GetComponent<Button>();

        // Store the original color of the button
        _originalColor = _button.colors.normalColor;

        // Add listener to button to send selected session
        _button.onClick.AddListener(() =>
        {
            // Set selected session
            SessionMenu.OnSessionSelected.Invoke(transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
            Select();
        });
    }

    /// <summary>
    /// Sets the button's session address and updates the button text.
    /// </summary>
    /// <param name="session">The robot session to associate with the button.</param>
    public void SetButton(RobotSession session)
    {
        _sessionAddress = session.Address;

        // Set button text to session name
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _sessionAddress;
    }

    /// <summary>
    /// Makes the button appear selected by changing its color.
    /// </summary>
    public void Select()
    {
        // Make the button appear selected
        _button.GetComponent<Image>().color = _originalColor * 0.74f;
    }

    /// <summary>
    /// Resets the button's color to its original state, indicating it is not selected.
    /// </summary>
    public void Deselect()
    {
        _button.GetComponent<Image>().color = _originalColor;
    }
}