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