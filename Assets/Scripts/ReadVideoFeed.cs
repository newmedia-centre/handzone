using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadVideoFeed : MonoBehaviour
{
    // Assign with the Material component of your object where you want to display the video feed
    private Material _materialRef;

    private void Start()
    {
        _materialRef = GetComponent<Material>();
        
        // Subscribe to the OnCameraFeed event
        WebClient.OnCameraFeed += LoadVideoFeed;
    }

    void LoadVideoFeed(Texture2D texture2D)
    {
        _materialRef.mainTexture = texture2D;
    }
}
