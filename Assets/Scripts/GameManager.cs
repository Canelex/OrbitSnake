using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Managers
    private static GameManager instance;
    private AssetManager assetManager;
    private CanvasManager canvasManager;

    // Game variables
    [Header("Game Variables")]
    public float planetRadiusGrowth;
    public float planetKConstGrowth;
    public float planetSpawnDelay;
    [Header("Objects")]
    public Transform background;
    public Snake snake;
    private bool playing;
    private bool firstSpawned;
    private float score;
    [Header("Prefabs")]
    public Snake prefabSnake;
    public Planet prefabPlanet;
    public Popup prefabTipText;
    public Popup prefabTipImage;

    public static GameManager Instance
    {
        get { return instance; }
    }

    private void Update()
    {
        if (playing)
        {
            UpdatePlanets();
            snake.UpdateSnake();
            background.position = (Vector2)snake.transform.position; // Move stars behind Snake

            // Update score
            score += 5 * Time.deltaTime;
            canvasManager.GetField("Score").text.text = GetScore().ToString(); // UI
        }

        Helper.cameraFollow.UpdateCamera();
    }

    private void UpdatePlanets()
    {
        Planet[] planets = Helper.planets;
        System.Array.Sort(planets, ComparePlanets);

        // Update planets isPressed state
        foreach (Touch touch in Input.touches)
        {
            bool touchGrowingPlanet = false;
            Vector3 point = Camera.main.ScreenToWorldPoint(touch.position);

            foreach (Planet planet in planets)
            {
                if (touch.phase == TouchPhase.Ended && planet.IsPressed(touch.fingerId))
                {
                    planet.Unpress(); // Finger released planet.
                }

                if (!touchGrowingPlanet && touch.phase == TouchPhase.Began)
                {
                    if (!planet.IsPressed() && planet.OverPlanet(point))
                    {
                        planet.Press(touch.fingerId); // Finger clicked on this planet
                        touchGrowingPlanet = true;
                    }
                }
            }
        }

        // Grow planets we need to grow (max 1 per finger)
        bool growingPlanets = false;
        float dt = Time.deltaTime;
        foreach (Planet planet in planets)
        {
            if (planet.IsPressed())
            {
                planet.kConst += planetKConstGrowth * dt;
                planet.radius += planetRadiusGrowth * dt;
                planet.transform.localScale = Vector3.one * planet.radius;
                growingPlanets = true; // at least one planet growing (sound!)
            }
        }

        // Play sound
        if (growingPlanets && !assetManager.IsSoundPlaying())
        {
            assetManager.PlaySound("Blop"); // Planet growing sound
        }
    }

    private void SpawnPlanet()
    {
        if (!playing) return;

        Vector3 position = snake.transform.position;
        float ahead = 10;
        position += snake.transform.up * ahead;

        if (!firstSpawned)
        {
            // Position
            bool leftOrRight = Random.Range(-1F, 1F) < 0;
            position += snake.transform.right * (leftOrRight ? -1 : 1) * Random.Range(2F, 2.5F);

            // Popup text
            Popup prfb = Instantiate(prefabTipText, canvasManager.GetPage("Game").obj.transform);
            prfb.position = position + Vector3.down * 1.5F;
            prfb.GetComponent<Text>().text = assetManager.GetLocalizedString("tip_text");

            // Popup image
            prfb = Instantiate(prefabTipImage, canvasManager.GetPage("Game").obj.transform);
            prfb.position = position;

            firstSpawned = true;
        }
        else
        {
            position += snake.transform.right * Random.Range(-6F, 6F);
        }

        // Make sure we are not nearby another planet.
        foreach (Planet planet in FindObjectsOfType<Planet>())
        {
            if (Vector3.Distance(planet.transform.position, position) < 2.5F)
            {
                return;
            }
        }

        Instantiate(prefabPlanet, position, Quaternion.identity);
    }

    public void DestroyAllPlanets()
    {
        Planet[] planets = FindObjectsOfType<Planet>();
        foreach (Planet planet in planets)
        {
            Destroy(planet.gameObject);
        }
    }

    public void StartGame()
    {
        // Open correct UI
        canvasManager.CloseAllPages();
        canvasManager.ShowPage("Game");

        // Reset map
        DestroyAllPlanets();

        // Reset Snake
        // TODO skins
        snake = Instantiate(prefabSnake, Vector3.zero, Quaternion.identity);
        CameraFollow follow = Helper.cameraFollow;
        follow.transform.position = Vector3.back * 10;
        follow.transform.rotation = Quaternion.identity;
        follow.SetFollowing(snake.transform);

        InvokeRepeating("SpawnPlanet", 0F, planetSpawnDelay);

        // Start the game.
        score = 0;
        playing = true;
        firstSpawned = false;
    }

    public void StopGame()
    {
        // Stop the game
        playing = false;

        int score = GetScore();
        Prefs.SetInt("last_score_points", score);
        if (score > Prefs.GetInt("best_score_points", 0)) // Update high score
        {
            Prefs.SetInt("best_score_points", score);
        }

        UpdateUI(); // Update fields
        CancelInvoke("SpawnPlanet"); // Stop spawning
        Invoke("ShowGameOver", 1F); // Show UI in 1s
    }

    private void ShowGameOver()
    {
        canvasManager.CloseAllPages(); // Close all pages
        canvasManager.ShowPage("Menu"); // Open game over
    }

    private void UpdateUI()
    {
        int bestScore = Prefs.GetInt("best_score_points", 0);
        int lastScore = Prefs.GetInt("last_score_points", 0);

        // Update game over canvas
        canvasManager.GetField("Last Score").text.text = lastScore.ToString();
        canvasManager.GetField("Best Score").text.text = bestScore.ToString();
    }

    private int ComparePlanets(Planet a, Planet b)
    {
        if (a.kConst > b.kConst) return 1;
        if (a.kConst < b.kConst) return -1;
        return 0;
    }

    public int GetScore()
    {
        return (int)score;
    }

    private void Start()
    {
        // Initialize variables
        instance = this;
        assetManager = AssetManager.Instance;
        canvasManager = CanvasManager.Instance;

        assetManager.PlayMusic("Music");
        assetManager.UpdateAudio();
        UpdateUI(); // Update UI (one time)
    }
}
