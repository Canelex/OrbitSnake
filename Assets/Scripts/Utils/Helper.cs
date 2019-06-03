using UnityEngine;

public class Helper
{
    public static float screenHeight
    {
        get {return Camera.main.pixelHeight;}
    }

    public static float screenWidth
    {
        get {return Camera.main.pixelWidth;}
    }

    public static float worldHeight
    {
        get {return Camera.main.orthographicSize;}
    }

    public static float worldWidth
    {
        get {return Camera.main.aspect * worldHeight;}
    }

    public static CameraFollow cameraFollow
    {
        get {return GameObject.FindObjectOfType<CameraFollow>();}
    }

    public static GameManager gameManager
    {
        get {return GameObject.FindObjectOfType<GameManager>();}
    }

    public static Planet[] planets
    {
        get {return GameObject.FindObjectsOfType<Planet>();}
    }

    public static GameObject[] objects
    {
        get {return GameObject.FindGameObjectsWithTag("SpaceObject");}
    }
}