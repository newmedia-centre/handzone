using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassHopperObject : MonoBehaviour
{
    private void Update()
    {
        if (transform.hasChanged)
        {
            UnityInGrasshopper.instance.SendPositionRotation(
                transform.position, 
                transform.eulerAngles, name);

            transform.hasChanged = false;
        }
    }
}
