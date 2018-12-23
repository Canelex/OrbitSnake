using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Level Settings (editor)
    public int targetFrameRate;
    public float cameraWidth;
    [Space(20)]
    public float levelScrollSpeed;
    public float rocketSpeed;
    public float maxRotateSpeed;
    public float gravityConstant;
    [Space(20)]
    public float planetMassGrowth;
    public float planetScaleGrowth;
    public float planetSpawnGap;
    public Planet planetPrefab;
    private RocketController rocket;
    private CanvasController canvas;
    private float screenWidth; // SetupScene
    private float screenHeight; // SetupScene

    // Level variables (this run)
    public string activeSkinName;
    private Skin activeSkin;
    private List<Skin> unlockedSkins;
    private bool levelPlaying;
    private int stage;
    private bool stageDir;
    private float distance;
    private float distSincePlanet;

    private void Start()
    {
        canvas = FindObjectOfType<CanvasController>();
        SetupScene(); // Camera and framerate
    }

    private void Update()
    {
        if (levelPlaying)
        {
            float dt = Time.deltaTime;

            // Move scrolling objects down.
            float levelScrollDist = levelScrollSpeed * dt;
            Vector2 levelScrollVec = Vector2.down * levelScrollDist;
            Scrolling[] scrolling = FindObjectsOfType<Scrolling>();
            foreach (Scrolling obj in scrolling) 
            {
                obj.Scroll(levelScrollVec);
            }

            // Grow planets.
            Planet[] planets = FindObjectsOfType<Planet>();
            foreach (Planet planet in planets)
            {
                if (planet.IsPressed())
                {
                    planet.ChangeMassAndScale(planetMassGrowth * dt, planetScaleGrowth * dt);
                }
            }

            distance += levelScrollDist;

            // Spawn planet
            distSincePlanet += levelScrollDist;
            if (distSincePlanet > planetSpawnGap)
            {
                distSincePlanet -= planetSpawnGap;
                SpawnPlanet();
            }
        }
    }

    private void SetupScene()
    {
        // Set frame rate
        if (Application.targetFrameRate != targetFrameRate)
        {
            Application.targetFrameRate = targetFrameRate;
        }

        // Set up the camera
        Camera cam = Camera.main;
        screenWidth = cameraWidth;
        screenHeight = screenWidth / cam.aspect;
        cam.orthographicSize = screenHeight;
    }

    public void StartLevel()
    {
        activeSkin = FindObjectOfType<SkinManager>().GetSkinByName(activeSkinName);

        // Load skin and setup rocket.
        if (rocket != null)
        {
            Destroy(rocket.gameObject);
        }
        rocket = Instantiate(activeSkin.rocket);
        rocket.Reset();

        // Destroy all planets
        DestroyAllPlanets();

        // Level variables
        stage = 0;
        levelPlaying = true;
        distance = 0;
        unlockedSkins = new List<Skin>(); // empty
    }

    public void StopLevel()
    {
        // Level variables
        levelPlaying = false;

        // Delete player and show canvas soon
        canvas.ToggleGame(false, 0.75F);
        canvas.ToggleGameOver(true, 0.75F);

        // Update high score?
        if (GetDistance() > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", GetDistance());
        }

        // Unlocked skin? Show that (canvas)
        if (unlockedSkins.Count > 0)
        {

        }

        // Show main menu (canvas)
    }
    
    private void SpawnPlanet()
    {
        float axis = screenWidth / 2; // -2.5, 0, 2.5
        float range = Random.Range(-screenWidth / 8, screenWidth / 8); // -2 2
        Vector2 pos = Vector2.zero;

        switch (stage)
        {
            case 0:
                pos = new Vector2(-axis + range, 10);
                stage++;
                break;
            case 1:
                pos = new Vector2(range, 10);
                stage++;
                break;
            case 2:
                pos = new Vector2(axis + range, 10);
                stage++;
                break;
            default: // case 3
                pos = new Vector2(range, 10);
                stage = 0;
                break;
        }

        Planet planet = Instantiate(planetPrefab, pos, Quaternion.identity);
    }

    public void DestroyAllPlanets()
    {
        Planet[] planets = FindObjectsOfType<Planet>();
        foreach (Planet planet in planets)
        {
            Destroy(planet.gameObject);
        }
    }

    public float GetScreenWidth()
    {
        return screenWidth;
    }

    public float GetScreenHeight()
    {
        return screenHeight;
    }

    public int GetDistance()
    {
        return (int) distance;
    }

    public bool IsLevelPlaying()
    {
        return levelPlaying;
    }
}
