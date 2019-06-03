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
    public bool playing;
    private bool firstSpawned;
    private bool gameOverShowed;
    public float score;
    [Header("Prefabs")]
    public Snake prefabSnake;
    public Planet prefabPlanet;
    public Popup prefabTipText;
    public Popup prefabTipImage;
    public Powerup[] prefabPowerups;

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
        else if (!Shutters.Instance.LoadingScene && gameOverShowed)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Shutters.Instance.LoadSceneWithShutters("Menu");
                    return;
                }
            }
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
            else if (planet.Distance(snake.transform.position) > 24)
            {
                Destroy(planet.gameObject);
            }
        }

        // Play sound
        if (growingPlanets && !assetManager.IsSoundPlaying())
        {
            assetManager.PlaySound("Blop"); // Planet growing sound
        }
    }

    private void SpawnPlanets()
    {
        Vector2 position = snake.transform.position; // 13 - 8 = 5

        float radian = 0;
        while (radian < Mathf.PI * 2)
        {
            float radius = Random.Range(9F, 13F);
            Vector2 relative = new Vector2(Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius);
            if (Mathf.Abs(relative.x) < Helper.worldWidth + 1 && Mathf.Abs(relative.y) < Helper.worldHeight + 1) continue;
            SpawnPlanet(position + relative);
            radian += Random.Range(0F, Mathf.PI / 9F);
        }
    }

    private void SpawnPlanet(Vector2 position)
    {
        int randomChance = Random.Range(0, 100);

        if (!firstSpawned)
        {
            if (position.y < 0 || Mathf.Abs(position.x) < 2.0 || Mathf.Abs(position.x) > 3.5)
            {
                return;
            }

             // Popup text
            Popup text = Instantiate(prefabTipText, canvasManager.GetPage("Game").obj.transform);
            text.position = position + Vector2.down * 1.5F;
            text.GetComponent<Text>().text = assetManager.GetLocalizedString("tip_text");

            // Popup image
            Popup outline = Instantiate(prefabTipImage, canvasManager.GetPage("Game").obj.transform);
            outline.position = position;

            firstSpawned = true;
            randomChance = 100;
        }

        foreach (GameObject go in Helper.objects)
        {
            if (Vector2.Distance(position, go.transform.position) < 3.0F)
            {
                return;
            }
        }

        if (randomChance < 10) { // 20 percent chance planet = poweurp
            Powerup powerup = prefabPowerups[Random.Range(0, prefabPowerups.Length)];
            Instantiate(powerup, position, Quaternion.identity);
            return;
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

        InvokeRepeating("SpawnPlanets", 0F, planetSpawnDelay);

        // Start the game.
        score = 0;
        playing = true;
        firstSpawned = false;
    }

    public void StopGame()
    {
        // Stop the game
        playing = false;

        // Update scores
        int scoreEarned = GetScore();
        Prefs.lastScore = scoreEarned;
        Prefs.credits += scoreEarned; // Give credits
        if (scoreEarned > Prefs.bestScore)
        {
            Prefs.bestScore = scoreEarned;
        }

        UpdateUI(); // Update fields
        CancelInvoke("SpawnPlanets"); // Stop spawning
        Invoke("ShowGameOver", 1F); // Show UI in 1s
    }

    private void ShowGameOver()
    {
        canvasManager.CloseAllPages(); // Close all pages
        canvasManager.ShowPage("GameOver"); // Open game over
        gameOverShowed = true;
    }

    private void UpdateUI()
    {
        // Update game over canvas
        canvasManager.GetField("Last Score").text.text = Prefs.lastScore.ToString();
        canvasManager.GetField("Best Score").text.text = Prefs.bestScore.ToString();
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
        firstSpawned = false;
        assetManager = AssetManager.Instance;
        canvasManager = CanvasManager.Instance;

        UpdateUI(); // Update UI (one time)

        StartGame();
    }
}
