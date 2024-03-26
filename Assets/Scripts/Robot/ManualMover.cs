using UnityEngine;

public class ManualMover : MonoBehaviour
{
    [Header("References")]
    public RobotTranslator translator;
    public GameObject transparentRobotGripper;
    public GameObject robotRoot;

    public void selected()
    {
        //transparentRobotGripper.SetActive(true);
    }
    public void deSelected()
    {
        //double[] target = new double[6];
        //target[0] = transform.position.x - robotRoot.transform.position.x;
        //target[1] = transform.position.y - robotRoot.transform.position.y;
        //target[2] = transform.position.z - robotRoot.transform.position.z;
        //target[3] = transform.rotation.eulerAngles.x;
        //target[4] = transform.rotation.eulerAngles.y;
        //target[5] = transform.rotation.eulerAngles.z;

        //translator.UpdateFromInverseKinematics(target, () => transparentRobotGripper.SetActive(false));
    }
}
