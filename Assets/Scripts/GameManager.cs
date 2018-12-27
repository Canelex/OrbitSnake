using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int targetFrameRate;
    public float cameraWidth;
    public string activeSkinName;
    [Space(20)]
    public float distanceTeleport;
    public float planetMassGrowth;
    public float planetScaleGrowth;
    public float planetSpawnDelay;
    private Camera mainCamera;
    private Rocket rocket;
    private CanvasController canvas;
    [Space(20)]
	public Transform starfield1;
	public Transform starfield2;
    public Planet planetPrefab;
    public ParticleSystem teleportEffect;
	public string gamemode;
    private bool gamePlaying;
    private float score;

    private void Update()
    {
        if (gamePlaying)
        {
            // Update rocket
            rocket.UpdateRocket();

            // Move stars behind rocket
            starfield1.position = (Vector2) mainCamera.transform.position;
			starfield2.position = (Vector2) mainCamera.transform.position;

            // Update planets
            float dt = Time.deltaTime;
            Planet[] planets = FindObjectsOfType<Planet>();
            System.Array.Sort(planets, CompareMasses); // Prioritize smaller masses
            bool growingPlanet = false;
            foreach (Planet planet in planets)
            {
                planet.UpdatePlanet();

                if (!growingPlanet && planet.IsPressed())
                {
                    planet.ChangeMassAndScale(planetMassGrowth * dt, planetScaleGrowth * dt);
                    growingPlanet = true;
                }
            }

			if (gamemode == "Points")
			{
				// Update score
            	score += 5 * dt;
			}
        }
    }

	private int CompareMasses(Mass a, Mass b)
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
        Vector3 position = rocket.transform.position;
        position += rocket.transform.up * 8;

        if (GetScore() < 50)
        {
            bool leftOrRight = Random.Range(-1F, 1F) < 0;
            position += rocket.transform.right * (leftOrRight ? -1 : 1) * Random.Range(0.5F, 3F);
        }
        else
        {
            position += rocket.transform.right * Random.Range(-3F, 3F);
        }

        Planet planet = Instantiate(planetPrefab, position, Quaternion.identity);
    }

	public void ShakeCamera(float time)
	{
		StartCoroutine(CameraShake(time));
	}

    public void DestroyAllPlanets()
    {
        Planet[] planets = FindObjectsOfType<Planet>();
        foreach (Planet planet in planets)
        {
            Destroy(planet.gameObject);
        }
    }

    public void TeleportForward(GameObject button)
    {
        if (gamePlaying)
        {
            rocket.transform.Translate(0, distanceTeleport, 0);
            //Debug.Break();

            foreach (Planet planet in FindObjectsOfType<Planet>()) // Trails
            {
                ParticleSystem ps = Instantiate(teleportEffect, planet.transform.position, rocket.transform.rotation);
                ParticleSystem.MainModule main = ps.main;
                main.startColor = planet.GetPrimaryColor();
                ParticleSystem.ShapeModule shape = ps.shape;
                shape.radius = 288F / 384F / 2 * planet.transform.localScale.x;
                Destroy(ps.gameObject, 2);
            }

            StartCoroutine(CameraShake(0.25F)); // Shake camera
        }
    }

    public void StartGame()
    {
        Skin activeSkin = FindObjectOfType<SkinManager>().GetSkinByName(activeSkinName);

        // Load skin and setup rocket.
        if (rocket != null) Destroy(rocket.gameObject);
        rocket = Instantiate(activeSkin.rocket);
        rocket.transform.position = Vector3.zero;
        rocket.transform.rotation = Quaternion.identity;

        // Reset camera
        Transform camera = Camera.main.transform;
        camera.parent = rocket.transform;
        camera.localPosition = new Vector3(0, 0, camera.position.z);
        camera.localRotation = Quaternion.identity;
        camera.localScale = Vector3.one;

        // Reset map
        DestroyAllPlanets();

        // Start Spawning
        InvokeRepeating("SpawnPlanet", 0F, planetSpawnDelay);

        // Start the game.
        score = 0;
        gamePlaying = true;
    }

    public void StopGame()
    {
        // Detatch camera
        Camera.main.transform.parent = null;

		// Stop the game.
        gamePlaying = false;

        if (GetScore() > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", GetScore());
        }

		// Stop Spawning
        CancelInvoke("SpawnPlanet");

		// Show canvas
        canvas.ToggleGame(false, 1.0F);
        canvas.ToggleGameOver(true, 1.0F);
    }

    private void Start()
    {
        // Set the application frame rate
        if (Application.targetFrameRate != targetFrameRate)
        {
            Application.targetFrameRate = targetFrameRate;
        }

        // Set the camera size to match width instead of height
        mainCamera = Camera.main;
        float cameraHeight = cameraWidth / mainCamera.aspect;
        mainCamera.orthographicSize = cameraHeight;

        // Generate planet sprites
        PlanetTextures.Instance.GenerateSprites();

        // Find the canvas object
        canvas = FindObjectOfType<CanvasController>();

        // Start fading planet outlines (one time)
        StartCoroutine(FadePlanetOutlines(0.5F));
    }

	private IEnumerator CameraShake(float time)
    {
        Transform camera = Camera.main.transform;
        Transform rock = rocket.transform;
        float factor = 2;

        for (float t = time; t >= 0; t -= Time.deltaTime)
        {
            float ft = factor * t;
            float dx = Random.Range(-ft, ft);
            float dy = Random.Range(-ft, ft);
            camera.position = new Vector3(rock.position.x + dx, rock.position.y + dy, camera.position.z);
            yield return null;
        }

        camera.position = new Vector3(rock.position.x, rock.position.y, camera.position.z);
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
}
