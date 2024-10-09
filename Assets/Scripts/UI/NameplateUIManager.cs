using System;
using System.Collections.Generic;
using UnityEngine;

public class NameplateUIManager : MonoBehaviour
{
    [Serializable]
    public struct NameplateStruct
    {
        public GameObject target;
        public string label;
        public Vector3 offset;

        public NameplateStruct(GameObject target, string label, Vector3 offset)
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

    private void Start()
    {
        _nameplateUIs = new List<NameplateUI>();
        
        // Build nameplates on awake
        foreach (var nameplate in nameplates)
        {
            var nameplateUI = Instantiate(nameplatePrefab, nameplate.target.transform).GetComponent<NameplateUI>();
            nameplateUI.DisplayLabel = nameplate.label;
            nameplateUI.Offset = nameplate.offset;
            nameplateUI.Target = nameplate.target;
            
            _nameplateUIs.Add(nameplateUI);
        }

        HideNameplates();
    }

    private void Update()
    {
        if(_nameplateUIs.Count == 0)
            return;

        foreach (var nameplate in nameplates)
        {
            var correspondingUI = _nameplateUIs.Find(ui => ui.Target == nameplate.target);
        
            if (correspondingUI != null)
            {
                correspondingUI.DisplayLabel = nameplate.label;
                correspondingUI.Offset = nameplate.offset;
            }
        }
    }

    // Adds new nameplate to the scene and adds it to the nameplates list
    public void AddNameplateToObject(GameObject target, string displayLabel, Vector3 offset)
    {
        var nameplateUI = Instantiate(nameplatePrefab, target.transform).GetComponent<NameplateUI>();
        nameplateUI.DisplayLabel = displayLabel;
        nameplateUI.Offset = offset;
        nameplateUI.Target = target;
        
        nameplates.Add(new NameplateStruct(target, displayLabel, offset));
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

    // Access a specific nameplate by target
    public NameplateUI GetNameplateByTarget(GameObject target)
    {
        return _nameplateUIs.Find(ui => ui.Target == target);
    }

    // Example method to control a specific nameplate
    public void ShowNameplate(GameObject target)
    {
        var nameplateUI = GetNameplateByTarget(target);
        if (nameplateUI != null)
        {
            nameplateUI.Show();
        }
    }

    public void HideNameplate(GameObject target)
    {
        var nameplateUI = GetNameplateByTarget(target);
        if (nameplateUI != null)
        {
            nameplateUI.Hide();
        }
    }
}