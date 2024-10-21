using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class VisibilityClip : PlayableAsset
{
    public ExposedReference<GameObject> targetObject;
    public bool isVisible;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VisibilityBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.targetObject = targetObject.Resolve(graph.GetResolver());
        behaviour.isVisible = isVisible;
        return playable;
    }
}