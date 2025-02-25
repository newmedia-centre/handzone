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
/// The NetworkVNCCursor class manages the behavior and appearance of a virtual cursor
/// in a networked environment. It is responsible for tracking the cursor's position,
/// updating its visual representation, and synchronizing its state across multiple clients in a multiplayer session. The class allows for interaction with virtual objects and provides feedback to users based on their actions.
/// </summary>
public class NetworkVNCCursor : MonoBehaviour
{
    [SerializeField] private Image coloredCursor;
    [SerializeField] private TMP_Text playerName;

    /// <summary>
    /// The color of the cursor, which can be customized for different players.
    /// </summary>
    public Color Color
    {
        set => coloredCursor.color = value;
    }

    /// <summary>
    /// The label that displays the player's name associated with the cursor.
    /// </summary>
    public string PlayerNameLabel
    {
        set => playerName.text = value;
    }
}