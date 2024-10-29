using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

[TrackColor(1, 0, 0)]
[TrackBindingType(typeof(VideoPlayer))]
[TrackClipType(typeof(VideoControlClip))]
public class VideoControlTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        foreach (TimelineClip clip in GetClips())
        {
            VideoControlClip vcClip = clip.asset as VideoControlClip;
        }

        return base.CreateTrackMixer(graph, go, inputCount);
    }
}