using UnityEngine;
using TMPro;

public class NameplateUI : MonoBehaviour
{
    public Vector3 Offset = new(0.08f, 0.0f, 0.0f);
    public GameObject Target;
    public string DisplayLabel
    {
        set => tmp.text = value;
    }
    
    [SerializeField] private string displayLabel = "Joint";
    [SerializeField] private TextMeshPro tmp;
    
    private bool _isShowing = true;

    // Update is called once per frame
    void Update()
    {
        if (_isShowing == false)
            return;
        
        // Calculate the local position with offset
        Vector3 localPosition = Target.transform.TransformPoint(Offset);
    
        // Set the world position of the nameplate
        transform.position = localPosition;
        
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _isShowing = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _isShowing = false;
    }
}
