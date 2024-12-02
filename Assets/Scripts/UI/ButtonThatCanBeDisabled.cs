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

using UnityEngine.Scripting;
using UnityEngine.UIElements;

#endregion

[Preserve]
public class ButtonThatCanBeDisabled : Button
{
    public bool enabled
    {
        get => enabledSelf;
        set => SetEnabled(value);
    }

    public new class UxmlFactory : UxmlFactory<ButtonThatCanBeDisabled, UxmlTraits>
    {
    }

    public new class UxmlTraits : Button.UxmlTraits
    {
        private UxmlBoolAttributeDescription enabledAttr = new() { name = "enabled", defaultValue = true };

        public override void Init(VisualElement ve, IUxmlAttributes attributes, CreationContext context)
        {
            base.Init(ve, attributes, context);
            var instance = (ButtonThatCanBeDisabled)ve;
            instance.enabled = enabledAttr.GetValueFromBag(attributes, context);
        }
    }
}