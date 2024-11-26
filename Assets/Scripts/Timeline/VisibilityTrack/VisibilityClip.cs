using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class VisibilityClip : PlayableAsset
{
    public ExposedReference<GameObject> targetObject;
    public GameObject transformGizmoPrefab;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VisibilityBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.transformGizmoPrefab = transformGizmoPrefab;
        behaviour.targetObject = targetObject.Resolve(graph.GetResolver());
        return playable;
    }
}