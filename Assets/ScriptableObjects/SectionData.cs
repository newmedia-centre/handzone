using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SectionData", order = 1)]
public class SectionData : ScriptableObject
{
    public string title;
    public string description;
    public bool completed;
    public TimelineAsset timelineAsset;
}
