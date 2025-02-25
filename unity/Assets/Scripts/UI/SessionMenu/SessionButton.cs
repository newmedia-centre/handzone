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