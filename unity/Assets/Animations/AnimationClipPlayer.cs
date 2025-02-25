using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class AnimationClipPlayer : MonoBehaviour
{
    public AnimationClip clip;
    
    private PlayableGraph _playableGraph;

    private void OnEnable()
    {
        AnimationPlayableUtilities.PlayClip(GetComponent<Animator>(), clip, out _playableGraph);
    }

    private void OnDisable()
    {
        // Destroy the graph when the script is disabled
        _playableGraph.Destroy();
    }
}
