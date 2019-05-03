using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float planetRadiusGrowth;
    public float planetKConstGrowth;
    public float planetSpawnDelay;
    public float distanceTeleport;
    public float rechargeTime;
    [Space(20)]
    public Transform stars;
    private CanvasManager canvas;
    private Snake snake;
    private bool playing;
    private bool firstSpawned;
    private float score;
    [Space(20)]
    // Prefabs
    public Snake prefabSnake;
    public Planet prefabPlanet;
    public ParticleSystem prefabTeleport;
    public ParticleSystem prefabExplosion;
    public Popup prefabPopup;

    private void Update()
    {
        if (playing)
        {
            // Update Snake
            snake.UpdateSnake();

            // Move stars behind Snake
            stars.position = (Vector2)snake.transform.position;

            // Update planets
            float dt = Time.deltaTime;
            Planet[] planets = Helper.planets;
            System.Array.Sort(planets, ComparePlanets); // Prioritize smaller masses
            bool growingPlanet = false;
            foreach (Planet planet in planets)
            {
                planet.UpdatePlanet();

                if (!growingPlanet && planet.IsPressed())
                {
                    planet.radius += planetRadiusGrowth * dt;
                    planet.kConst += planetKConstGrowth * dt;
                    growingPlanet = true;
                }
            }

            if (growingPlanet)
            {
                if (!SoundManager.Instance.IsSoundPlaying())
                {
                    SoundManager.Instance.PlaySound("Blop");
                }
            }

            // Update score
            score += 5 * dt;
            canvas.GetField("Score").text.text = GetScore().ToString();
        }

        Helper.cameraFollow.UpdateCamera();
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

    private void SpawnPlanet()
    {
        if (!playing) return;

        Vector3 position = snake.transform.position;
        float ahead = Mathf.Max(9F, Camera.main.orthographicSize + 1);
        position += snake.transform.up * ahead;

        if (!firstSpawned)
        {
            bool leftOrRight = Random.Range(-1F, 1F) < 0;
            position += snake.transform.right * (leftOrRight ? -1 : 1) * Random.Range(2F, 2.5F);
            firstSpawned = true;
            ShowTip(position + Vector3.down * 1.5F, "Tap and hold");
        }
        else
        {
            position += snake.transform.right * Random.Range(-5F, 5F);
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

    private void ShowTip(Vector3 position, string text)
    {
        Transform panel = canvas.GetPage("Game").obj.transform;
        Popup popup = Instantiate(prefabPopup, panel);
        popup.text = text;
        popup.position = position;
    }

    public void StartGame()
    {
        // Open correct UI
        canvas.CloseAllPages();
        canvas.ShowPage("Game");

        // Reset map
        DestroyAllPlanets();

        // Reset Snake
        // todo skins
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

        foreach (Planet planet in Helper.planets)
        {
            planet.DisableOutline();
        }

        Prefs.SetInt("last_score_points", GetScore());

        // Update high score
        if (GetScore() > Prefs.GetInt("best_score_points", 0))
        {
            Prefs.SetInt("best_score_points", GetScore());
        }

        UpdateUI(); // Update fields
        CancelInvoke("SpawnPlanet"); // Stop spawning
        Invoke("ShowGameOver", 1F); // Show UI in 1s
    }

    private void ShowGameOver()
    {
        canvas.CloseAllPages(); // Close all pages
        canvas.ShowPage("Menu"); // Open game over
    }

    private IEnumerator FadePlanetOutlines(float period)
    {
        bool fadingIn = true;

        for (float time = 0; ; time += Time.deltaTime)
        {
            if (time >= period)
            {
                time -= period;
                fadingIn = !fadingIn;
            }

            foreach (Planet planet in FindObjectsOfType<Planet>())
            {
                planet.FadeIcon(fadingIn, time / period);
            }

            yield return null;
        }
    }

    private void UpdateUI()
    {
        int bestScore = Prefs.GetInt("best_score_points", 0);
        int lastScore = Prefs.GetInt("last_score_points", 0);

        // Update game over canvas
        canvas.GetField("Last Score").text.text = lastScore.ToString();
        canvas.GetField("Best Score").text.text = bestScore.ToString();
    }

    private void Start()
    {
        // Initialize variables
        instance = this;
        canvas = CanvasManager.instance;

        UpdateUI(); // Update UI (one time)
        StartCoroutine(FadePlanetOutlines(0.5F)); // Start fading planet outlines (one time)
        SoundManager.Instance.PlayMusic("Music");
    }
}
