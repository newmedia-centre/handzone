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
            UnityInGrasshopper.instance.SendPosition(
                transform.position.x, 
                transform.position.y, 
                transform.position.z, name);
            
            transform.hasChanged = false;
        }
    }
}
