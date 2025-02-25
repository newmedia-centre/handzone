// Copyright 2024 NewMedia Centre - Delft University of Technology
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#region

using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

#endregion

/// <summary>
/// This class is a custom grab transformer that allows for rotation of the object when grabbed with a single Interactor.
/// The rotation is constrained to a single axis, and the object will rotate around the direction of that Interactor.
/// </summary>
public class XRSingleGrabRotationTransformer : XRBaseGrabTransformer
{
    private Vector3 _InitialGrabLocation;

    public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase,
        ref Pose targetPose, ref Vector3 localScale)
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
        var direction = thisTransformPose.position - interactorAttachPose.position;

        // Set the targetPose rotation to look towards the direction
        targetPose.rotation = Quaternion.LookRotation(direction, grabInteractable.transform.up);
    }
}