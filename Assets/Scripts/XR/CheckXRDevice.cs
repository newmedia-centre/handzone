using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckXRDevice : MonoBehaviour
{
    // Check for a VR Device at startup, if none is found activate the movement model
    bool VRCheck;
    public GameObject XR;
    public bool Override;
    void Start()
    {
        VRCheck = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head).isValid;
        Debug.Log("VR Device: " + VRCheck);
        if (VRCheck == false && Override == false)
        {
            XR.SetActive(false);
        }
    }
}
