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
using UnityEngine;

#endregion

public static class RobotActions
{
    public static Action OnToolLoaded;
    public static Action OnToolUnloaded;
    public static Action<GameObject> OnToolGrabbed;
    public static Action<GameObject> OnToolUngrabbed;
    public static Action<float> OnTimeUpdated;
    public static Action<int> OnProgramDurationUpdated;
}