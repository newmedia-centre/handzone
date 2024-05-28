using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class HighlightControlBehaviour : PlayableBehaviour
{
    [SerializeField]
    private Outline.Mode mode;

    [SerializeField]
    private Color color;

    [SerializeField]
    [Range(0f, 10f)]
    private float width;
    
    private GameObject obj;

    private bool firstFrameHappaned;
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        obj = playerData as GameObject;

        if (obj == null)
        {
            return;
        }

        if (!firstFrameHappaned)
        {
            Outline _outline = obj.AddComponent<Outline>();
            
            _outline.OutlineMode = mode;
            _outline.OutlineColor = color;
            _outline.OutlineWidth = width;
            
            firstFrameHappaned = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (obj == null)
        {
            return;
        }

        if (info.effectivePlayState == PlayState.Paused || playable.GetDuration() <= playable.GetTime())
        {
            // Debug.Log("Pause");
            // Debug.Log("State: " + info.effectivePlayState);
            // Debug.Log("Playable State: " + playable.IsDone());
            // Debug.Log("Evaluation Type: " + info.evaluationType);

            firstFrameHappaned = false;

            if (Application.isEditor)
            {
                UnityEngine.Object.DestroyImmediate(obj.GetComponent<Outline>());
            }
            else
            {
                UnityEngine.Object.Destroy(obj.GetComponent<Outline>());
            }
        }
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (obj == null)
        {
            return;
        }
    
        firstFrameHappaned = false;
        
        if (Application.isEditor)
        {
            UnityEngine.Object.DestroyImmediate(obj.GetComponent<Outline>());
        }
        else
        {
            UnityEngine.Object.Destroy(obj.GetComponent<Outline>());
        }
    }
}
