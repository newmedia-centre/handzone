using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class NameplateUI : MonoBehaviour
{
    public string displayLabel = "Joint";
    public Canvas nameplateCanvas;
    public GameObject nameplatePrefab;
    public Vector3 offset = new(0.08f, 0.0f, 0.0f);
    
    private GameObject _uiNameplate;
    private TextMeshProUGUI _textLabel;
    private Vector3 _centerPosition;
    private Renderer _renderer;
    
    void Start()
    {
        _uiNameplate = Instantiate(nameplatePrefab, nameplateCanvas.transform);
        _uiNameplate.GetComponentInChildren<TextMeshProUGUI>().text = displayLabel;
        _renderer = transform.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _centerPosition = _renderer.bounds.center;
        _uiNameplate.transform.position = _centerPosition + offset;
        _uiNameplate.transform.rotation = Quaternion.LookRotation(_uiNameplate.transform.position - Camera.main.transform.position);
    }
}
