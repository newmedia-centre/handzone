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