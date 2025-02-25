using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ChapterData", order = 1)]
public class ChapterData : ScriptableObject
{
    public string chapterName;
    public string description;
    public List<SectionData> sections;
}
