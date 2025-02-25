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

#endregion

/// <summary>
/// The PolyscopeMoveJointUI class manages the user interface for controlling
/// the movement of robotic joints. It initializes
/// the UI elements, assigns joint indices to the corresponding UI components,
/// and ensures that the UI reflects the current state of the robotic joints.
/// The class also handles warnings when the required components are not assigned.
/// </summary>
public class PolyscopeMoveJointUI : MonoBehaviour
{
    public RobotManager robotManager;

    /// <summary>
    /// Initializes the UI components and assigns joint indices to the
    /// PolyscopeMoveJointElement instances. This method is called when
    /// the script instance is being loaded.
    /// </summary>
    private void Start()
    {
        if (robotManager == null)
        {
            Debug.LogWarning("Cannot render Move Joint UI, RobotTranslator is not assigned");
            return;
        }

        var polyscopeMoveElements = GetComponentsInChildren<PolyscopeMoveJointElement>();
        for (var i = 0; i < polyscopeMoveElements.Length; i++) polyscopeMoveElements[i].jointIndex = i;
    }
}