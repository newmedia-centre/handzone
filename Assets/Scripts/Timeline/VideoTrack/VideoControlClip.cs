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
using UnityEngine.Video;

#endregion

[Serializable]
public class VideoControlClip : PlayableAsset, ITimelineClipAsset
{
    public VideoClip clip; // Field to hold the video clip
    public VideoClipType type; // Field to hold the video clip type

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VideoControlBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.clip = clip; // Assign the video clip to the behaviour
        behaviour.type = type; // Assign the video clip type to the behaviour

        // Set the duration of the playable to the duration of the video clip
        if (clip != null) playable.SetDuration(clip.length);

        return playable;
    }

    public ClipCaps clipCaps => ClipCaps.All;
}