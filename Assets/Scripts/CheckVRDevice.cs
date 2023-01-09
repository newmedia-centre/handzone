using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CheckVRDevice : MonoBehaviour
{
    // Check for a VR Device at startup, if none is found activate the movement model
    bool VRCheck;
    void Start()
    {
        VRCheck = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head).isValid;
        Debug.Log("VR Device: " + VRCheck);
        if (VRCheck == true)
        {
            Destroy(gameObject);
        }
    }
}
