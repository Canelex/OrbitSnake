using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float planetRadiusGrowth;
    public float planetMassGrowth;
    public float planetSpawnDelay;
    public float distanceTeleport;
    public float rechargeTime;
    public float G;
    public float D;
    [Space(20)]
    public Transform stars;
    private CanvasManager canvas;
    private Rocket rocket;
    private bool playing;
    private float score;
    [Space(20)]
    // Prefabs
    public Rocket prefabRocket;
    public Planet prefabPlanet;
    public ParticleSystem prefabTeleport;
    public ParticleSystem prefabExplosion;

    private void Update()
    {
        if (playing)
        {
            // Update rocket
            rocket.UpdateRocket();

            // Move stars behind rocket
            stars.position = (Vector2)rocket.transform.position;

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
                    planet.radius += (planetRadiusGrowth * dt);
                    planet.mass += planetMassGrowth * dt;
                    growingPlanet = true;
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
        if (a.GetMass() > b.GetMass()) return 1;
        if (a.GetMass() < b.GetMass()) return -1;
        return 0;
    }

    public int GetScore()
    {
        return (int)score;
    }

    private void SpawnPlanet()
    {
        if (!playing) return;

        Vector3 position = rocket.transform.position;
        position += rocket.transform.up * (Camera.main.orthographicSize + 1);

        if (GetScore() < 50)
        {
            bool leftOrRight = Random.Range(-1F, 1F) < 0;
            position += rocket.transform.right * (leftOrRight ? -1 : 1) * Random.Range(1F, 4F);
        }
        else
        {
            position += rocket.transform.right * Random.Range(-5F, 5F);
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

    public void TeleportForward()
    {
        if (playing)
        {
            rocket.transform.Translate(0, distanceTeleport, 0);

            foreach (Planet planet in Helper.planets) // Trails
            {
                ParticleSystem ps = Instantiate(prefabTeleport, planet.transform.position, rocket.transform.rotation);
                ParticleSystem.MainModule main = ps.main;
                main.startColor = planet.GetPrimaryColor();
                ParticleSystem.ShapeModule shape = ps.shape;
                shape.radius = 288F / 384F / 2 * planet.transform.localScale.x;
                Destroy(ps.gameObject, 2);
            }

            Helper.cameraFollow.SetShaking(0.5F, 0.5F); // Mild shaking effect

            StartCoroutine(RechargeButton()); // Recharge
        }
    }

    public void StartGame()
    {
        // Open correct UI
        canvas.CloseAllPages();
        canvas.ShowPage("Game");

        // Reset map
        DestroyAllPlanets();

        // Reset rocket
        // todo skins
        rocket = Instantiate(prefabRocket, Vector3.zero, Quaternion.identity);
        CameraFollow follow = Helper.cameraFollow;
        follow.transform.position = Vector3.back * 10;
        follow.transform.rotation = Quaternion.identity;
        follow.SetRocket(rocket.transform);

        // Start Spawning
        InvokeRepeating("SpawnPlanet", 0F, planetSpawnDelay);

        // Start the game.
        score = 0;
        playing = true;
    }

    public void StopGame()
    {
        // Stop the game
        playing = false;

        foreach (Planet planet in Helper.planets)
        {
            planet.DisableOutline();
        }

        PlayerPrefs.SetInt("LastScorePoints", GetScore());

        // Update high score
        if (GetScore() > PlayerPrefs.GetInt("BestScorePoints", 0))
        {
            PlayerPrefs.SetInt("BestScorePoints", GetScore());
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

    private IEnumerator RechargeButton()
    {
        CanvasManager.Field button = canvas.GetField("Boost");
        button.image.gameObject.SetActive(false);
        button.text.gameObject.SetActive(true);

        for (float time = 0; time <= rechargeTime && playing; time += Time.deltaTime)
        {
            float timeLeft = rechargeTime - time;
            button.text.text = string.Format("{0:0.0}", timeLeft);
            yield return null;
        }

        button.image.gameObject.SetActive(true);
        button.text.gameObject.SetActive(false);
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
        int bestScore = PlayerPrefs.GetInt("BestScorePoints", 0);
        int lastScore = PlayerPrefs.GetInt("LastScorePoints", 0);

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
    }
}
