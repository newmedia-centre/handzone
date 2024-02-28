using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class NameplateUIManager : MonoBehaviour
{
    [Serializable]
    public struct NameplateStruct
    {
        public GameObject target;
        public string label;
        public Vector3 offset;

        public NameplateStruct( GameObject target, string label, Vector3 offset)
        {
            this.target = target;
            this.label = label;
            this.offset = offset;
        }
    }

    public GameObject nameplatePrefab;
    [SerializeField] 
    public List<NameplateStruct> nameplates;

    private List<NameplateUI> _nameplateUIs;
    private Canvas _canvas;


    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _nameplateUIs = new List<NameplateUI>();
        
        // Build nameplates on awake
        foreach (var nameplate in nameplates)
        {
            var nameplateUI = nameplate.target.AddComponent<NameplateUI>();
            nameplateUI.nameplateCanvas = _canvas;
            nameplateUI.nameplatePrefab = nameplatePrefab;
            nameplateUI.displayLabel = nameplate.label;
            nameplateUI.offset = nameplate.offset;
            
            _nameplateUIs.Add(nameplateUI);
        }
    }

    // Adds new nameplate to the scene and adds it to the nameplates list
    public void AddNameplateToObject(GameObject gameObject, string displayLabel, Vector3 offset)
    {
        var nameplateUI = gameObject.AddComponent<NameplateUI>();
        nameplateUI.nameplateCanvas = _canvas;
        nameplateUI.nameplatePrefab = nameplatePrefab;
        nameplateUI.displayLabel = displayLabel;
        nameplateUI.offset = offset;
        
        nameplates.Add(new NameplateStruct(gameObject, displayLabel, offset));
        _nameplateUIs.Add(nameplateUI);
    }

    // Shows all nameplates 
    public void ShowNameplates()
    {
        foreach (var nameplateUI in _nameplateUIs)
        {
            nameplateUI.Show();
        }
    }

    // Hides all nameplates
    public void HideNameplates()
    {
        foreach (var nameplateUI in _nameplateUIs)
        {
            nameplateUI.Hide();
        }
    }
}

