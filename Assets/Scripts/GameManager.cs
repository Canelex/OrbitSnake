using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	// Scene Settings
	public int targetFrameRate;
    public float cameraWidth;
	public string activeSkinName;
    [Space(20)]
	// Game Settings
	public float planetMassGrowth;
    public float planetScaleGrowth;
    public float planetSpawnDelay;
    public Planet planetPrefab;
	private float screenWidth; // SetupScene
    private float screenHeight; // SetupScene
	private Rocket rocket;
	private CanvasController canvas;
	// Game Variables (run)
	private Skin activeSkin;
	public bool gamePlaying; 
	private float score;

    void Start()
    {
		canvas = FindObjectOfType<CanvasController>();
		SetupScene(); // Camera and framerate
    }

    void Update()
    {
		if (gamePlaying)
		{
			rocket.UpdateRocket();

			foreach (Starfield sf in FindObjectsOfType<Starfield>())
			{
				sf.UpdateStarfield();
			}

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

			score += 5 * dt;
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

	public void StartGame()
	{
		activeSkin = FindObjectOfType<SkinManager>().GetSkinByName(activeSkinName);

		// Reset camera
		Transform camera = Camera.main.transform;
		camera.position = new Vector3(0, 0, camera.position.z);
		camera.rotation = Quaternion.identity;

		// Load skin and setup rocket.
        if (rocket != null) Destroy(rocket.gameObject);
        rocket = Instantiate(activeSkin.rocket);
		rocket.transform.position = Vector3.zero;
		rocket.transform.rotation = Quaternion.identity;

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
		// Show canvas
		canvas.ToggleGame(false, 0.75F);
        canvas.ToggleGameOver(true, 0.75F);

		// Stop Spawning
		CancelInvoke("SpawnPlanet");

		if (GetScore() > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", GetScore());
        }

		// Stop the game.
		gamePlaying = false;
	}

	private void SpawnPlanet()
	{
		Vector3 position = rocket.transform.position;
		position += rocket.transform.up * Random.Range(6, 8);
		bool leftOrRight = Random.Range(-1F, 1F) < 0;
		position += rocket.transform.right * Random.Range(1, 3) * (leftOrRight ? -1 : 1);
		Planet planet = Instantiate(planetPrefab, position, Quaternion.identity);
	}

	private int CompareMasses(Mass a, Mass b)
	{
		if (a.GetMass() > b.GetMass()) return 1;
		if (a.GetMass() < b.GetMass()) return -1;
		return 0;
	}

    public void DestroyAllPlanets()
    {
        Planet[] planets = FindObjectsOfType<Planet>();
        foreach (Planet planet in planets)
        {
            Destroy(planet.gameObject);
        }
    }

	public int GetScore()
	{
		return (int) score;
	}
}
