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
        double totalDuration = 0.0d;
        
        foreach (TimelineClip clip in GetClips())
        {
            VideoControlClip vcClip = clip.asset as VideoControlClip;
            
            if (vcClip != null)
            {
                vcClip.ClipStart = clip.start;
                vcClip.ClipEnd = clip.end;
                vcClip.durationUsed = totalDuration;
                
                if (vcClip.template.type == VideoClipType.Default || vcClip.template.type == VideoClipType.ContinueLastFrame)
                {
                    totalDuration = 0.0d;
                }
                else if (vcClip.template.type != VideoClipType.HoldLastFrame)
                {
                    totalDuration += clip.end - clip.start;
                }

            }
        }
        
        return base.CreateTrackMixer(graph, go, inputCount);
    }
}
