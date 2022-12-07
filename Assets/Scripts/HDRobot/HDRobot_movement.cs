using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDRobot_movement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject.Find("HDRobot_0").transform.position = GameObject.Find("Joint 0").transform.position;
        GameObject.Find("HDRobot_0").transform.rotation = GameObject.Find("Joint 0").transform.rotation;

        GameObject.Find("HDRobot_1").transform.position = GameObject.Find("Joint 1").transform.position;
        GameObject.Find("HDRobot_1").transform.rotation = GameObject.Find("Joint 1").transform.rotation;

        GameObject.Find("HDRobot_2").transform.position = GameObject.Find("Joint 2").transform.position;
        GameObject.Find("HDRobot_2").transform.rotation = GameObject.Find("Joint 2").transform.rotation;

        GameObject.Find("HDRobot_3").transform.position = GameObject.Find("Joint 3").transform.position;
        GameObject.Find("HDRobot_3").transform.rotation = GameObject.Find("Joint 3").transform.rotation;

        GameObject.Find("HDRobot_4").transform.position = GameObject.Find("Joint 4").transform.position;
        GameObject.Find("HDRobot_4").transform.rotation = GameObject.Find("Joint 4").transform.rotation;

        GameObject.Find("HDRobot_5").transform.position = GameObject.Find("Joint 5").transform.position;
        GameObject.Find("HDRobot_5").transform.rotation = GameObject.Find("Joint 5").transform.rotation;

        GameObject.Find("HDRobot_6").transform.position = GameObject.Find("Joint 6").transform.position;
        GameObject.Find("HDRobot_6").transform.rotation = GameObject.Find("Joint 6").transform.rotation;
    }
}
