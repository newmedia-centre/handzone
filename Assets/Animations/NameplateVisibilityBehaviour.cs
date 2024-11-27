using UnityEngine;
using UnityEngine.Playables;

public class NameplateVisibilityBehaviour : PlayableBehaviour
{
    public bool isVisible = true;
    public GameObject nameplateObject;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (nameplateObject != null)
        {
            var nameplateUIManager = GameObject.FindObjectOfType<NameplateUIManager>();
            if (nameplateUIManager != null)
            {
                double currentTime = playable.GetTime();
                double duration = playable.GetDuration();
                bool shouldBeVisible = currentTime >= 0 && currentTime <= duration;

                if (shouldBeVisible && isVisible)
                {
                    nameplateUIManager.ShowNameplate(nameplateObject);
                }
                else
                {
                    nameplateUIManager.HideNameplate(nameplateObject);
                }
            }
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (nameplateObject != null)
        {
            var nameplateUIManager = GameObject.FindObjectOfType<NameplateUIManager>();
            if (nameplateUIManager != null)
            {
                nameplateUIManager.HideNameplate(nameplateObject);
            }
        }
    }
}