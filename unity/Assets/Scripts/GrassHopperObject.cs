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

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

#endregion

[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Outline))]
public class GrassHopperObject : MonoBehaviour
{
    public bool sendPosition = true;
    public bool sendRotation = true;
    public float updateDuration = 3;
    public Color selectableColor = Color.green;
    public Color unselectableColor = Color.red;
    public Color interactedColor = Color.cyan;

    private const int SCALE = 1000;
    private Timer _timer;
    private bool _shouldUpdate = false;
    private XRGrabInteractable _xrInteractable;
    private Outline _selectableOutline;
    private Outline _meshOutline;
    private Transform _meshTransform;
    private Rigidbody _meshRigidbody;
    private bool _previousMeshKinematicSetting;

    private void Awake()
    {
        _timer = GetComponent<Timer>();
        _timer.SetTimerDuration(updateDuration);
        _xrInteractable = GetComponent<XRGrabInteractable>();
        _selectableOutline = GetComponent<Outline>();
        _selectableOutline.OutlineColor = selectableColor;

        _meshTransform = transform.GetChild(0);
        _meshOutline = _meshTransform.GetComponent<Outline>();
        _meshRigidbody = _meshTransform.GetComponent<Rigidbody>();
        _meshOutline.OutlineColor = selectableColor;
    }

    private void OnEnable()
    {
        RobotActions.OnToolGrabbed += MeshUnselectable;
        RobotActions.OnToolUngrabbed += MeshSelectable;
        _xrInteractable.selectEntered.AddListener(delegate
        {
            DisablePhysics();
            ResetMesh();
            ShouldUpdate(true);
        });
        _xrInteractable.selectExited.AddListener(delegate
        {
            ShouldUpdate(false);
            EnablePhysics();
            // UnityInGrasshopper.Instance?.SendPosition(transform.position * SCALE, name);
        });
    }

    private void EnablePhysics()
    {
        _meshRigidbody.velocity = Vector3.zero;
        _meshRigidbody.ResetInertiaTensor();
        _meshRigidbody.isKinematic = false;
        _meshRigidbody.useGravity = true;
    }

    private void DisablePhysics()
    {
        _meshRigidbody.isKinematic = true;
        _meshRigidbody.useGravity = false;
    }

    private void Update()
    {
        if (transform.hasChanged && !_timer.Started() && _shouldUpdate)
        {
            if (!sendPosition && !sendRotation)
                return;

            if (sendPosition)
            {
                // UnityInGrasshopper.Instance?.SendPosition(transform.position * SCALE, name);
            }

            if (sendRotation)
            {
                // UnityInGrasshopper.Instance?.SendRotationQuaternion(transform.rotation, name);
            }

            transform.hasChanged = false;
            _timer.StartTimer();
        }
    }

    public void ShouldUpdate(bool value)
    {
        _shouldUpdate = value;
    }

    public void MeshUnselectable(GameObject go)
    {
        DisablePhysics();

        if (go == _meshTransform.gameObject)
        {
            _meshOutline.OutlineColor = unselectableColor;
            _selectableOutline.OutlineColor = interactedColor;
            _meshRigidbody.isKinematic = _previousMeshKinematicSetting;
        }
    }

    public void MeshSelectable(GameObject go)
    {
        EnablePhysics();

        if (go == _meshTransform.gameObject)
        {
            _meshOutline.OutlineColor = selectableColor;
            _selectableOutline.OutlineColor = selectableColor;
            // _meshRigidbody.isKinematic = _previousMeshKinematicSetting;
            // if (_meshRigidbody.isKinematic)
            // {
            //     _previousMeshKinematicSetting = true;
            // }
            // else
            // {
            //     _previousMeshKinematicSetting = false;
            //     _meshRigidbody.isKinematic = true;
            // }
        }
    }

    private void ResetMesh()
    {
        _meshTransform.SetPositionAndRotation(transform.position, transform.rotation);
    }
}