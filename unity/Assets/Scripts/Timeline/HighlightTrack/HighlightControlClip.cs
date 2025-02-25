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
using UnityEngine.Playables;
using UnityEngine.Timeline;

#endregion

/// <summary>
/// Represents a playable asset that controls highlighting effects on target GameObjects.
/// </summary>
[Serializable]
public class HighlightControlClip : PlayableAsset, ITimelineClipAsset
{
    public ExposedReference<GameObject> targetObjects;
    public Outline.Mode mode;
    public Color color;
    [Range(0f, 10f)] public float width;

    /// <summary>
    /// Creates a playable for this clip.
    /// </summary>
    /// <param name="graph">The playable graph to create the playable in.</param>
    /// <param name="owner">The GameObject that owns the playable.</param>
    /// <returns>A playable that represents this clip.</returns>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<HighlightControlBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.targetObject = targetObjects.Resolve(graph.GetResolver());
        behaviour.mode = mode;
        behaviour.color = color;
        behaviour.width = width;
        return playable;
    }

    public ClipCaps clipCaps => ClipCaps.None;
}