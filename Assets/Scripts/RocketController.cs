using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    private GameController game;
    public ParticleSystem explosion;
    private float rotation;

    private void Start()
    {
        game = FindObjectOfType<GameController>();
    }

    private void Update()
    {
        if (game.IsLevelPlaying())
        {
            // Rotate the rocket depending on forces.
            float fNetX = GetFNetX();
            float turnAngle = Mathf.Clamp(-fNetX, -game.maxRotateSpeed, game.maxRotateSpeed) * Time.deltaTime;
            transform.Rotate(0, 0, turnAngle);

            // Translate the player according to rotation.
            float xMove = -Mathf.Sin(transform.rotation.z * Mathf.Deg2Rad) * game.rocketSpeed;
            transform.Translate(xMove, -transform.position.y, 0);

            if (Mathf.Abs(transform.position.x) > game.GetScreenWidth())
            {
                Crash(transform.position);
            }
        }
    }

    private float GetFNetX()
    {
        Planet[] planets = FindObjectsOfType<Planet>();
        Vector2 fNet = Vector2.zero;
        Vector2 rocketPos = transform.position;

        foreach (Planet planet in planets)
        {
            Vector2 planetPos = planet.transform.position;
            float distance = Vector3.Distance(planetPos, rocketPos);
            Vector2 direction = Vector3.Normalize(planetPos - rocketPos);
            fNet += (planet.planetMass * direction) / (distance * distance);
        }

        fNet *= game.gravityConstant;
        return fNet.x;
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        // Find contact point
        Vector2 average = Vector2.zero;
        foreach (ContactPoint2D point in coll.contacts)
        {
            average += point.point;
        }
        average /= coll.contacts.Length;
        Crash(average);
    }

    private void Crash(Vector3 position)
    {
        // Shake effect
        StartCoroutine(RocketShrink(0.25F));
        StartCoroutine(CameraShake(Camera.main, 0.5F));

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Explosion effect
        ParticleSystem ps = Instantiate(explosion, position, Quaternion.identity);
        Destroy(ps.gameObject, 5F);

        // Stop level
        game.StopLevel();
    }

    private IEnumerator RocketShrink(float time)
    {
        for (float t = time; t >= 0; t -= Time.deltaTime)
        {
            float percent = t / time;
            transform.localScale = Vector3.one * percent;
            yield return null;
        }

        transform.localScale = Vector3.zero;
    }

    private IEnumerator CameraShake(Camera cam, float time)
    {
        float factor = 2;

        for (float t = time; t >= 0; t -= Time.deltaTime)
        {
            float ft = factor * t;
            cam.transform.position = new Vector3(Random.Range(-ft, ft), Random.Range(-ft ,ft), cam.transform.position.z);
            yield return null;
        }

        cam.transform.position = new Vector3(0, 0, cam.transform.position.z);
    }

    public void Reset()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    public float GetRotationRadians()
    {
        return rotation;
    }
}
