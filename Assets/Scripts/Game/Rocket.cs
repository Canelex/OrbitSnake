using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float rocketSpeed;
    public GameObject emission;
    public ParticleSystem explosion;
    private GameManager game;

    public void UpdateRocket()
    {
        // Calculate turn
        Vector3 anet = GetNetAcceleration();
        float radiansCurrent = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        float radiansFinal = Mathf.Atan2(-anet.x, anet.y);
        float radiansToTurn = radiansFinal - radiansCurrent;

        // Wrap it
        while (radiansToTurn < -Mathf.PI) radiansToTurn += Mathf.PI * 2;
        while (radiansToTurn > Mathf.PI) radiansToTurn -= Mathf.PI * 2;

        // Movement
        transform.Rotate(0, 0, radiansToTurn * Mathf.Rad2Deg * anet.magnitude * Time.deltaTime);
        transform.Translate(0, rocketSpeed * Time.deltaTime, 0);
    }

    private Vector2 GetNetAcceleration()
    {
        Vector3 anet = Vector2.zero;

        foreach (Planet planet in Helper.planets)
        {
            Vector3 direction = Vector3.Normalize(planet.transform.position - transform.position);
            float distance = Vector3.Distance(planet.transform.position, transform.position) * game.D;//distancePerWorldUnit;
            anet += direction * (game.G * planet.GetMass() / Mathf.Pow(distance, 2));
        }

        return anet;
    }

    public void ResetRocket()
    {
        // Reset transform
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // Enable emissions
        emission.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.tag == "Planet")
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
    }

    private void Crash(Vector3 position)
    {
        // Stop level
        game.StopGame();

        // Explosion effect
        ParticleSystem ps = Instantiate(explosion, position, Quaternion.identity);
        Destroy(ps.gameObject, 5F);
        Helper.cameraFollow.SetShaking(1F, 1F);

        // Hide the rocket
        StartCoroutine(RocketShrink(0.25F));

        // Hide emissions
        emission.SetActive(false);
    }

    private IEnumerator RocketShrink(float time)
    {
        // Shrink rocket scale to zero
        for (float t = time; t >= 0; t -= Time.deltaTime)
        {
            float percent = t / time;
            transform.localScale = Vector3.one * percent;
            yield return null;
        }

        transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        game = FindObjectOfType<GameManager>();
    }
}
