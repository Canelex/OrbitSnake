using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Mass
{
	public float rocketSpeed;
	private GameManager game;
	public ParticleSystem explosion;

    private void Start()
    {
		game = FindObjectOfType<GameManager>();
		Camera.main.transform.parent = transform;
    }

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
		// Detatch camera.
		Camera.main.transform.parent = null;

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
        game.StopGame();
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
		Vector3 start = cam.transform.position;
        float factor = 2;

        for (float t = time; t >= 0; t -= Time.deltaTime)
        {
            float ft = factor * t;
            cam.transform.position = start + new Vector3(Random.Range(-ft, ft), Random.Range(-ft ,ft));
            yield return null;
        }

        cam.transform.position = start;
    }
}
