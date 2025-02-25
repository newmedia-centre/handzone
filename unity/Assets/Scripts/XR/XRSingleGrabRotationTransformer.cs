/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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