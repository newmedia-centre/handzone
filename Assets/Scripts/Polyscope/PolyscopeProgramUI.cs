using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyscopeProgramUI : MonoBehaviour
{
    public enum Feature
    {
        View = 0,
        Base = 1,
        Tool = 2
    }

    public Feature currentFeature = Feature.Base;
    
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }
}
