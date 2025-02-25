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

using UnityEngine.Playables;

#endregion

/// <summary>
/// A behaviour that processes the mixing of highlight control playables.
/// </summary>
public class HighlightControlMixerBehaviour : PlayableBehaviour
{
    /// <summary>
    /// Processes each frame of the playable.
    /// </summary>
    /// <param name="playable">The playable being processed.</param>
    /// <param name="info">Frame data for the current frame.</param>
    /// <param name="playerData">Data associated with the player.</param>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var inputCount = playable.GetInputCount();

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);
            if (inputWeight > 0.5f)
            {
                var inputPlayable = playable.GetInput(i);
                if (inputPlayable.GetPlayableType() == typeof(ScriptPlayable<HighlightControlBehaviour>))
                {
                    var visibilityPlayable = (ScriptPlayable<VisibilityBehaviour>)inputPlayable;
                    var visibilityBehaviour = visibilityPlayable.GetBehaviour();
                    visibilityBehaviour.ProcessFrame(playable, info, playerData);
                }
                else if (inputPlayable.GetPlayableType() ==
                         typeof(ScriptPlayable<TranslatableHighlightControlBehaviour>))
                {
                    var translatablePlayable = (ScriptPlayable<TranslatableHighlightControlBehaviour>)inputPlayable;
                    var translatableBehaviour = translatablePlayable.GetBehaviour();
                    translatableBehaviour.ProcessFrame(playable, info, playerData);
                }
            }
        }
    }
}