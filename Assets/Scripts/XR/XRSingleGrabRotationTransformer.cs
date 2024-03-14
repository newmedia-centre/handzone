using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

/// <summary>
/// This class is a custom grab transformer that allows for rotation of the object when grabbed with a single Interactor.
/// The rotation is constrained to a single axis, and the object will rotate around the direction of that Interactor.
/// </summary>
public class XRSingleGrabRotationTransformer : XRBaseGrabTransformer
{
    private Vector3 _InitialGrabLocation;
    
    public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
    {
        Debug.Log("Hi");
        switch (updatePhase)
        {
            case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
            case XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender:
            {
                UpdateTarget(grabInteractable, ref targetPose);

                break;
            }
        }
        
    }

    internal static void UpdateTarget(XRGrabInteractable grabInteractable, ref Pose targetPose)
    {
        var interactor = grabInteractable.interactorsSelecting[0];
        var interactorAttachPose = interactor.GetAttachTransform(grabInteractable).GetWorldPose();
        var thisTransformPose = grabInteractable.transform.GetWorldPose();

        // Calculate the direction from the interactor to the grabInteractable
        Vector3 direction = thisTransformPose.position - interactorAttachPose.position;

        // Set the targetPose rotation to look towards the direction
        targetPose.rotation = Quaternion.LookRotation(direction, grabInteractable.transform.up);
    }
}
