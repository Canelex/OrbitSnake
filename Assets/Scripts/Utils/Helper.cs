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

    public static bool soundEnabled
    {
        get {return Prefs.GetBool("sound_enabled", true);}
        set {Prefs.SetBool("sound_enabled", value);}
    }

    public static bool musicEnabled
    {
        get {return Prefs.GetBool("music_enabled", true);}
        set {Prefs.SetBool("music_enabled", value);}
    }
}