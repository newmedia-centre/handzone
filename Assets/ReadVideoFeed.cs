using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadVideoFeed : MonoBehaviour
{
    
    private RawImage _rawImage;

    private void Start()
    {
        _rawImage = GetComponent<RawImage>();
        WebClient.OnCameraFeed += LoadVideoFeed;
    }

    void LoadVideoFeed(Texture2D texture2D)
    {
        _rawImage.texture = texture2D;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
