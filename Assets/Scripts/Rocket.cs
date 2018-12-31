using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Mass
{
    public float rocketSpeed;
    public ParticleSystem explosion;
    private GameManager game;

    public void UpdateRocket()
    {
        // Calculate turn
        Vector3 fnet = GetNetForce();
        float radiansCurrent = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        float radiansFinal = Mathf.Atan2(-fnet.x, fnet.y);
        float radiansToTurn = radiansFinal - radiansCurrent;

        // Wrap it
        while (radiansToTurn < -Mathf.PI) radiansToTurn += Mathf.PI * 2;
        while (radiansToTurn > Mathf.PI) radiansToTurn -= Mathf.PI * 2;

        // Movement
        transform.Rotate(0, 0, radiansToTurn * Mathf.Rad2Deg * fnet.magnitude * Time.deltaTime);
        transform.Translate(0, rocketSpeed * Time.deltaTime, 0);
    }

    private Vector2 GetNetForce()
    {
        Vector3 fnet = Vector2.zero;

        foreach (Mass mass in FindObjectsOfType<Mass>())
        {
            if (mass == this) continue;

            Vector3 direction = Vector3.Normalize(mass.transform.position - transform.position);
            float distance = Vector3.Distance(mass.transform.position, transform.position) * Physics.distancePerWorldUnit;
            fnet += direction * (Physics.GConst * GetMass() * mass.GetMass() / Mathf.Pow(distance, 2));
        }

        return fnet;
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Astronaut")
        {
            Astronaut astronaut = collider.GetComponent<Astronaut>();
            if (!astronaut.collected)
            {
                game.SpawnAstronaut();
                game.score++;
                astronaut.FlyAtRocket(transform);
                astronaut.collected = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
    }

    private void Crash(Vector3 position)
    {
        // Detatch camera
        Camera.main.transform.parent = null;

        // Stop level
        game.StopGame();

        // Explosion effect
        ParticleSystem ps = Instantiate(explosion, position, Quaternion.identity);
        Destroy(ps.gameObject, 5F);
        game.ShakeCamera(0.5F);

        // Make rocket invisible
        StartCoroutine(RocketShrink(0.25F));

        // Destroy exhaust
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void Start()
    {
        game = FindObjectOfType<GameManager>();
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
}
