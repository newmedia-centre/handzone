using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SectionData", order = 1)]
public class SectionData : ScriptableObject
{
    public string sectionName;
    public string description;
    public bool sectionCompleted;
}
