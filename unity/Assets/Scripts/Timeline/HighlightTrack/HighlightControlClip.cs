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