using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class CheckVRDevice : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        var VRCheck = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head).isValid;
        // Debug.Log(VRCheck);
    }
}
