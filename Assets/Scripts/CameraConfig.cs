using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraConfig : MonoBehaviour
{
    public float screenWidth = 4F;
    public int framerate = 60;

    private void Start()
    {
        // Setup camera
        Camera camera = GetComponent<Camera>();
        float screenHeight = screenWidth / camera.aspect;
        if (screenHeight >= 8) screenHeight = 8;
        camera.orthographicSize = screenHeight;    

        // Set framerate
        if (Application.targetFrameRate != framerate)
        {
            Application.targetFrameRate = framerate;
        }    
    }
}
