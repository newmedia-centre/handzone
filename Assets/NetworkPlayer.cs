using UnityEngine;
using XRZone;

public class NetworkPlayer : MonoBehaviour
{
    public string ID { get; set; }
    public Transform hmdTransform;
    public Transform leftTransform;
    public Transform rightTransform;
    public string playerName;
    public Renderer playerRenderer;
    

    public void SetPosition(PositionDataOut data)
    {
        //hmdTransform.transform.position.x = data.hmd.U;
    }
}
