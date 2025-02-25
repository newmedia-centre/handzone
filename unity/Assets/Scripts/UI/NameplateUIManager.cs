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
        if (_nameplateUIs == null) return;

        foreach (var nameplateUI in _nameplateUIs) nameplateUI.Show();
    }

    /// <summary>
    /// Hides all nameplates currently managed by this manager.
    /// </summary>
    public void HideNameplates()
    {
        if (_nameplateUIs == null) return;

        foreach (var nameplateUI in _nameplateUIs) nameplateUI.Hide();
    }

    /// <summary>
    /// Accesses a specific nameplate by its target game object.
    /// </summary>
    /// <param name="target">The game object whose nameplate is to be accessed.</param>
    /// <returns>The corresponding NameplateUI, or null if not found.</returns>
    public NameplateUI GetNameplateByTarget(GameObject target)
    {
        return _nameplateUIs?.Find(ui => ui.Target == target);
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