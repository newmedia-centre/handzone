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

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

/// <summary>
/// Manages the creation and visibility of nameplates for game objects in the scene.
/// </summary>
public class NameplateUIManager : MonoBehaviour
{
    [Serializable]
    public struct NameplateStruct
    {
        public GameObject target;
        public string label;
        public Vector3 offsetStart;
        public Vector3 offsetEnd;

        public NameplateStruct(GameObject target, string label, Vector3 offsetStart, Vector3 offsetEnd)
        {
            this.target = target;
            this.label = label;
            this.offsetStart = offsetStart;
            this.offsetEnd = offsetEnd;
        }
    }

    public GameObject nameplatePrefab;
    [SerializeField] public List<NameplateStruct> nameplates;

    private List<NameplateUI> _nameplateUIs;

    /// <summary>
    /// Initializes the nameplate UI manager and builds nameplates on awake.
    /// </summary>
    private void Start()
    {
        _nameplateUIs = new List<NameplateUI>();

        // Build nameplates on awake
        foreach (var nameplate in nameplates)
        {
            var nameplateUI = Instantiate(nameplatePrefab, nameplate.target.transform).GetComponent<NameplateUI>();
            nameplateUI.DisplayLabel = nameplate.label;
            nameplateUI.OffsetStart = nameplate.offsetStart;
            nameplateUI.OffsetEnd = nameplate.offsetEnd;
            nameplateUI.Target = nameplate.target;

            _nameplateUIs.Add(nameplateUI);
        }

        HideNameplates();
    }

    /// <summary>
    /// Updates the nameplates' properties based on the current state of the nameplates list.
    /// </summary>
    private void Update()
    {
        if (_nameplateUIs.Count == 0)
            return;

        foreach (var nameplate in nameplates)
        {
            var correspondingUI = _nameplateUIs.Find(ui => ui.Target == nameplate.target);

            if (correspondingUI != null)
            {
                correspondingUI.DisplayLabel = nameplate.label;
                correspondingUI.OffsetStart = nameplate.offsetStart;
                correspondingUI.OffsetEnd = nameplate.offsetEnd;
            }
        }
    }

    /// <summary>
    /// Adds a new nameplate to the specified game object and updates the nameplates list.
    /// </summary>
    /// <param name="target">The game object to which the nameplate is attached.</param>
    /// <param name="displayLabel">The label to display on the nameplate.</param>
    /// <param name="offsetStart">The starting offset for the nameplate.</param>
    /// <param name="offsetEnd">The ending offset for the nameplate.</param>
    public void AddNameplateToObject(GameObject target, string displayLabel, Vector3 offsetStart, Vector3 offsetEnd)
    {
        var nameplateUI = Instantiate(nameplatePrefab, target.transform).GetComponent<NameplateUI>();
        nameplateUI.DisplayLabel = displayLabel;
        nameplateUI.OffsetStart = offsetStart;
        nameplateUI.OffsetEnd = offsetEnd;
        nameplateUI.Target = target;

        nameplates.Add(new NameplateStruct(target, displayLabel, offsetStart, offsetEnd));
        _nameplateUIs.Add(nameplateUI);
    }

    /// <summary>
    /// Shows all nameplates currently managed by this manager.
    /// </summary>
    public void ShowNameplates()
    {
        foreach (var nameplateUI in _nameplateUIs) nameplateUI.Show();
    }

    /// <summary>
    /// Hides all nameplates currently managed by this manager.
    /// </summary>
    public void HideNameplates()
    {
        foreach (var nameplateUI in _nameplateUIs) nameplateUI.Hide();
    }

    /// <summary>
    /// Accesses a specific nameplate by its target game object.
    /// </summary>
    /// <param name="target">The game object whose nameplate is to be accessed.</param>
    /// <returns>The corresponding NameplateUI, or null if not found.</returns>
    public NameplateUI GetNameplateByTarget(GameObject target)
    {
        return _nameplateUIs.Find(ui => ui.Target == target);
    }

    /// <summary>
    /// Shows the nameplate associated with the specified target game object.
    /// </summary>
    /// <param name="target">The game object whose nameplate is to be shown.</param>
    public void ShowNameplate(GameObject target)
    {
        var nameplateUI = GetNameplateByTarget(target);
        if (nameplateUI != null) nameplateUI.Show();
    }

    /// <summary>
    /// Hides the nameplate associated with the specified target game object.
    /// </summary>
    /// <param name="target">The game object whose nameplate is to be hidden.</param>
    public void HideNameplate(GameObject target)
    {
        var nameplateUI = GetNameplateByTarget(target);
        if (nameplateUI != null) nameplateUI.Hide();
    }
}