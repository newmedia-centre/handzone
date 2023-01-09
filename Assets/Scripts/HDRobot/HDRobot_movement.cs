using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HDRobot_movement : MonoBehaviour
{    
    public GameObject LDRobot;

    bool ObjectFound = false;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ObjectFound == false)
        {
            if (LDRobot.transform.Find("Joint 0") != null)
            {
                ObjectFound = true;
                Renderer[] rs = LDRobot.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in rs)
                    r.enabled = false;
            }
            /*
            else
            {
                Debug.Log("Object not Found");
            } */
        }
        else
        {
            Debug.Log("Input Robot Model is Found");
            foreach (int joint in Enumerable.Range(0, 7))
            {
                transform.GetChild(joint).transform.position = LDRobot.transform.GetChild(joint).transform.position;
                transform.GetChild(joint).transform.rotation = LDRobot.transform.GetChild(joint).transform.rotation;
            }
        }
    }
}
