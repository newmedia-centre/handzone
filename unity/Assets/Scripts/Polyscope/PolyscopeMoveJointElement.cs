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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

/// <summary>
/// The PolyscopeMoveJointElement class manages the user interface elements
/// for controlling a robotic joint. It handles
/// the initialization of UI components such as sliders and input fields,
/// updates the joint's position based on user input, and responds to
/// changes in joint angles from the robot's state. The class also manages
/// the interaction with buttons that control the movement of the joint.
/// </summary>
public class PolyscopeMoveJointElement : MonoBehaviour
{
    public Button positiveButton;
    public Button negativeButton;
    public int jointIndex;

    private TMP_Text _jointLabel;
    private Slider _slider;
    private TMP_InputField _inputField;

    /// <summary>
    /// Initializes the UI components and sets up event listeners for user input
    /// and joint state changes. This method is called when the script instance
    /// is being loaded.
    /// </summary>
    private void Start()
    {
        _slider = GetComponentInChildren<Slider>();
        _inputField = GetComponentInChildren<TMP_InputField>();

        _inputField.onSubmit.AddListener(delegate { UpdateJoint(); });
        UR_EthernetIPClient.JointChanged += UpdateUI;

        // Add components to buttons and set their initial values
        var moveJointButton =
            positiveButton.gameObject.AddComponent(typeof(PolyscopeMoveJointButton)) as PolyscopeMoveJointButton;
        if (moveJointButton != null)
        {
            moveJointButton.jointIndex = jointIndex;
            moveJointButton.direction = 1;
        }

        // Change negative direction Vector to inverse for only this button
        moveJointButton =
            negativeButton.gameObject.AddComponent(typeof(PolyscopeMoveJointButton)) as PolyscopeMoveJointButton;
        if (moveJointButton != null)
        {
            moveJointButton.jointIndex = jointIndex;
            moveJointButton.direction = -1;
        }
    }

    /// <summary>
    /// Updates the UI elements based on the current state of the joint.
    /// This method is called when the joint's angle changes.
    /// </summary>
    /// <param name="jointIndex">The index of the joint being updated.</param>
    /// <param name="angle">The new angle of the joint.</param>
    private void UpdateUI(int jointIndex, float angle)
    {
        if (jointIndex == this.jointIndex)
        {
            angle *= Mathf.Rad2Deg;
            _slider.value = angle;
            _inputField.text = angle.ToString("F2");
        }
    }

    private void UpdateJoint()
    {
        if (float.TryParse(_inputField.text, out var angle)) _slider.value = angle;
        // RobotTranslator.UpdatePolyscopeJoint(jointIndex, angle);
        // TODO: CHange to use WebClient event
    }
}