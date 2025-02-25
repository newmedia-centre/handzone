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