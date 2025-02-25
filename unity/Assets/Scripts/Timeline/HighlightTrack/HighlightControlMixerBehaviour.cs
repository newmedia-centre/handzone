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