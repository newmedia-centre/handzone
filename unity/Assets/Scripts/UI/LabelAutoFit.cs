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
using UnityEngine.Scripting;
using UnityEngine.UIElements;

#endregion

// IMPORTANT NOTE:
// This element doesn't work with flexGrow as it leads to undefined behaviour (recursion).
// Use Size/Width[%] and Size/Height attributes instead

/// <summary>
/// Automatically adjusts the font size of a label based on its dimensions.
/// </summary>
[Preserve]
public class LabelAutoFit : Label
{
    public Axis axis { get; set; }
    public float ratio { get; set; }

    [Preserve]
    public new class UxmlFactory : UxmlFactory<LabelAutoFit, UxmlTraits>
    {
    }

    [Preserve]
    public new class UxmlTraits : Label.UxmlTraits // VisualElement.UxmlTraits
    {
        private UxmlFloatAttributeDescription _ratio = new()
        {
            name = "ratio",
            defaultValue = 0.1f,
            restriction = new UxmlValueBounds { min = "0.0", max = "0.9", excludeMin = false, excludeMax = true }
        };

        private UxmlEnumAttributeDescription<Axis> _axis = new()
        {
            name = "ratio-axis",
            defaultValue = Axis.Horizontal
        };

        /// <summary>
        /// Initializes the visual element with attributes from the UXML.
        /// </summary>
        /// <param name="ve">The visual element being initialized.</param>
        /// <param name="bag">The attribute bag containing UXML attributes.</param>
        /// <param name="cc">The creation context.</param>
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            var instance = ve as LabelAutoFit;
            instance.RegisterCallback<GeometryChangedEvent>(instance.OnGeometryChanged);

            instance.ratio = _ratio.GetValueFromBag(bag, cc);
            instance.axis = _axis.GetValueFromBag(bag, cc);
            instance.style.fontSize = 1; // triggers GeometryChangedEvent
        }
    }

    /// <summary>
    /// Called when the geometry of the label changes.
    /// Adjusts the font size based on the new dimensions.
    /// </summary>
    /// <param name="evt">The geometry changed event.</param>
    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        var oldRectSize = axis == Axis.Vertical ? evt.oldRect.height : evt.oldRect.width;
        var newRectLenght = axis == Axis.Vertical ? evt.newRect.height : evt.newRect.width;

        var oldFontSize = style.fontSize.value.value;
        var newFontSize = newRectLenght * ratio;

        var fontSizeDelta = Mathf.Abs(oldFontSize - newFontSize);
        var fontSizeDeltaNormalized = fontSizeDelta / Mathf.Max(oldFontSize, 1);

        if (fontSizeDeltaNormalized > 0.01f)
            style.fontSize = newFontSize;
    }

    /// <summary>
    /// Enum representing the axis for font size adjustment.
    /// </summary>
    public enum Axis
    {
        Horizontal,
        Vertical
    }
}