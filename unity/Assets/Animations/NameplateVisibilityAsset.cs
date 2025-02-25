using UnityEngine;
using UnityEngine.Playables;

public class NameplateVisibilityAsset : PlayableAsset
{
    public bool isVisible;
    public ExposedReference<GameObject> nameplateObject;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<NameplateVisibilityBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.isVisible = isVisible;
        behaviour.nameplateObject = nameplateObject.Resolve(graph.GetResolver());
        return playable;
    }
}