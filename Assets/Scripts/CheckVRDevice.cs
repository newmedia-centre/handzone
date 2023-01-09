using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CheckVRDevice : MonoBehaviour
{
<<<<<<< HEAD
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
=======

    // Update is called once per frame
    void Update()
    {
        var VRCheck = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head).isValid;
        // Debug.Log(VRCheck);
>>>>>>> 2a3d54676d190054490501723a07de2949f8ab10
    }
}
