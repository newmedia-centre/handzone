using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class GrassHopperObject : MonoBehaviour
{
    public bool SendPosition = true;
    public bool SendRotation = true;

    private const int SCALE = 1000;

    private void Update()
    {
        if (transform.hasChanged)
        {
            if (!SendPosition && !SendRotation)
                return;
             
            if (SendPosition)
            {
                UnityInGrasshopper.Instance.SendPosition(transform.position * SCALE, name);
            }
            if (SendRotation)
            {
                UnityInGrasshopper.Instance.SendRotationQuaternion(transform.rotation, name);
            }

            transform.hasChanged = false;
        }
    }
}
